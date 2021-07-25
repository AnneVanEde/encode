using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ENCODE.Base
{
    class Program
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        static private MSBuildWorkspace workspace;
        static private Microsoft.CodeAnalysis.Project project;
        static private Compilation compilation;

        static public Project projectStructure;
        static public CurrentPlanProfile planProfile;

        static public MenuForm menuForm;
        static public string projectPath;

        [STAThread]
        static async Task Main(string[] args)
        {
            // Create Console
            AllocConsole();

            // Create default planning profile
            planProfile = new CurrentPlanProfile();

            projectPath = @"C:\Users\avanede\Documents\DOTS-training-samples-master\Original\AntPhermones\Assembly-CSharp.csproj";

            // Create Window
            OpenMainWindow();

            if (Constant.DEBUG_OUTPUT)
            {
                // Prepare Project
                PrepareProject();

                // Plan the project
                PlanProject();

                // Input handling for viewing the results
                HandleUserInput();
            }
        }

        private static void OpenMainWindow()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            menuForm = new MenuForm();
            Application.Run(menuForm);

        }


        public static async Task PrepareProject()
        {

            // Create an empty structure to store everyting
            projectStructure = new Project();
            projectStructure.oodRawItems.AddRange(planProfile.KnownParentClasses);

            // Only interesting if there are multiple versions of VS installed
            menuForm.UpdateStepLabel("Roslyn APIs Analysis...");
            SetVisualStudioInstance();
            menuForm.UpdateStepProgressBar(33);

            // Prepare the Syntax and Semantic analysis
            await OpenProject();
            menuForm.UpdateStepProgressBar(66);

            await CompileProject();
            menuForm.UpdateStepProgressBar(100);
            menuForm.UpdateTotalProgressBar(10);

            // Parse, Link, Connect the project
            menuForm.UpdateStepLabel("DOC Code Analysis...");
            ParseProject();
            menuForm.UpdateStepProgressBar(33);

            LinkProject();
            menuForm.UpdateStepProgressBar(66);

            ConnectProject();
            menuForm.UpdateStepProgressBar(100);
            menuForm.UpdateTotalProgressBar(20);
        }

        #region Menu and User Input

        private static void HandleUserInput()
        {
            PrintMenu();
            Logger.Separator();

            while (true)
            {
                Logger.Write("> ");
                var readLine = Console.ReadLine();
                Logger.WriteLineToFileOnly(readLine);

                if (readLine == "-h")
                {
                    PrintMenu();
                    Logger.Separator();
                    continue;
                }

                string[] userResponse = readLine.Split('-');

                //if (userResponse[0] == "tadaa" && int.TryParse(userResponse[1], out int inputIndex))
                //{
                //    TreeWalker.PrintComponent(projectStructure, new IndexTuple((int)ArrayIndex.ECSComponent, inputIndex), 0);
                //    Logger.Separator();
                //    continue;
                //}

                if (!UserFormatValid(userResponse) || !UserInputParamsValid(userResponse, out string outputMode, out string inputType, out string inputParam, out IndexTuple indexTuple))
                    continue;

                int range = 3;

                if (outputMode == Constant.MENU_OUTPUT_MODE[1] && inputParam == Constant.MENU_INPUT_PARAM[0])
                {
                    Logger.WriteLine("You don't want to draw EVERYTING. Trust me. Choose something more specific.");
                    Logger.Separator();
                    continue;
                }
                else if (outputMode == Constant.MENU_OUTPUT_MODE[0]) // debug
                {
                    if (inputParam == Constant.MENU_INPUT_PARAM[0])
                    {
                        DebugAll(inputType);
                    }
                    else
                    {
                        // Raw Items View
                        DebugSubStructure(indexTuple);
                    }
                }
                else if (outputMode == Constant.MENU_OUTPUT_MODE[2]) // print
                {
                    if (inputParam == Constant.MENU_INPUT_PARAM[0])
                    {
                        DebugAll(inputType);
                    }
                    else
                    {
                        // Raw Items View
                        PrintSubStructure(indexTuple);
                    }
                }

                Logger.Separator();

                // --- Notes ---
                // draw-*-all           > is succesfulle ignored   
                // *-method-[name]      > cannot be done
                // *-component-[name]   > cannot be done
                // print-namespace-*    > because of fields, raw view is used: only one class in namespace is shown
                // depth                > depth for graphs not yet implemented
                // ood/raw              > only raw is possible now because of fields

                // print vs. debug?
            }
        }

        private static void PrintMenu()
        {
            Logger.WriteLine("What do you want to do: ");
            Logger.WriteLineToConsoleOnly("[output mode]-[input type]-[input parameter]");
            Logger.WriteLineToConsoleOnly("");

            Logger.WriteLineToConsoleOnly("Output mode options:");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_OUTPUT_MODE[0]} \t \t - debug raw view in console");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_OUTPUT_MODE[2]} \t \t - print ood view in console");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_OUTPUT_MODE[1]} \t \t - draw graph");
            Logger.WriteLineToConsoleOnly("");

            Logger.WriteLineToConsoleOnly("Input type options:");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[0]} \t - A single namespace with children");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[1]} \t \t - A single class with children");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[2]} \t \t - A single method with children"); // method searching on name not working yet
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[3]} \t - A superclass with its subclasses");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[4]} \t - An overridden method with its override methods");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[5]} \t - A component with its datafields");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_TYPE[6]} \t \t - A system with its components");
            Logger.WriteLineToConsoleOnly("");

            Logger.WriteLineToConsoleOnly("Input parameter options:");
            Logger.WriteLineToConsoleOnly("[namespace]  \t - the name of the desired namespace");
            Logger.WriteLineToConsoleOnly("[namespace:name] - the name of the desired class/classgroup");
            Logger.WriteLineToConsoleOnly("[index] \t - the index of the desired namespace/class/etc.");
            Logger.WriteLineToConsoleOnly($"{Constant.MENU_INPUT_PARAM[0]} \t \t - all namespaces/classes/etc.");
            Logger.WriteLineToConsoleOnly("");

            Logger.WriteLineToConsoleOnly("Example: print-class-all");
            Logger.WriteLineToConsoleOnly("Note: You can't search methods, systems or components by name");
            Logger.WriteLineToConsoleOnly("");

        }

        #endregion

        #region Input Validation

        private static bool UserFormatValid(string[] userResponse)
        {
            if (userResponse.Length <= 2)
            {
                Logger.WriteLine("Input not accepted, use the correct format. Input '-h' for input options");
                Logger.Separator();
                return false;
            }
            return true;
        }

        private static bool UserInputParamsValid(string[] userResponse, out string outputMode, out string inputType, out string inputParam, out IndexTuple indexTuple)
        {
            outputMode = userResponse[0];
            inputType = userResponse[1];
            inputParam = userResponse[2];

            indexTuple = new IndexTuple();

            bool outputModeValid = Constant.MENU_OUTPUT_MODE.Contains(outputMode);
            bool inputTypeValid = Constant.MENU_INPUT_TYPE.Contains(inputType);
            bool inputParamValid = Constant.MENU_INPUT_PARAM.Contains(inputParam) ||
                FoundItemInProject(inputType, inputParam, out indexTuple) ||
                (int.TryParse(inputParam, out int inputIndex) && FoundItemInProject(inputType, inputIndex, out indexTuple));


            if (!outputModeValid || !inputTypeValid || !inputParamValid)
            {
                string error = "Input not accepted";
                error += !outputModeValid ? ", [output mode] Invalid" : "";
                error += !inputTypeValid ? ", [input type] Invalid" : "";
                error += !inputParamValid ? ", [input parameter] Invalid" : "";
                Logger.WriteLine(error + ". Input '-h' for input options");
                Logger.Separator();
                return false;
            }

            return true;
        }

        private static bool FoundItemInProject(string inputType, string inputParameter, out IndexTuple indexTuple)
        {
            string type = inputType.First().ToString().ToUpper() + inputType.Substring(1);
            string name = inputParameter;
            OODItem oodItem = null;
            indexTuple = null;

            if (GenerateKey(inputType, inputParameter, out string key) && projectStructure.oodRawDictionary.TryGetValue(key, out indexTuple))
            {
                return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
            }

            return false;
        }

        private static bool GenerateKey(string inputType, string inputParameter, out string key)
        {
            string type = inputType.First().ToString().ToUpper() + inputType.Substring(1);
            string[] nameTag = inputParameter.Split(':');

            switch (type)
            {
                case nameof(Types.Class):
                    if (nameTag.Length > 1)
                    {
                        key = $"{type}_{nameTag[0]}_{nameTag[1]}";
                        return true;
                    }
                    break;
                case nameof(Types.ClassGroup):
                    if (nameTag.Length > 1 &&
                        projectStructure.oodRawDictionary.TryGetValue($"{Types.Class}_{nameTag[0]}_{nameTag[1]}", out IndexTuple indexTuple) &&
                        projectStructure.DOCListGetItem(indexTuple, out OODItem oodItem) == 1)
                    {
                        key = $"{Types.ClassGroup}_{oodItem.GetKey()}";
                        return true;
                    }
                    break;
                default:
                    key = $"{type}_{nameTag[0]}";
                    return true;
            }
            key = "";
            return false;
        }

        private static bool FoundItemInProject(string inputType, int inputParameter, out IndexTuple indexTuple)
        {
            string type = inputType.First().ToString().ToUpper() + inputType.Substring(1);
            OODItem oodItem;
            ECSItem ecsIem;

            switch (type)
            {
                case nameof(Types.Namespace):
                    indexTuple = new IndexTuple((int)Types.Namespace, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
                case nameof(Types.Class):
                    indexTuple = new IndexTuple((int)Types.Class, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
                case nameof(Types.Method):
                    indexTuple = new IndexTuple((int)Types.Method, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
                case nameof(Types.ClassGroup):
                    indexTuple = new IndexTuple((int)Types.ClassGroup, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
                case nameof(Types.MethodGroup):
                    indexTuple = new IndexTuple((int)Types.MethodGroup, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out oodItem) == 1;
                case nameof(Types.Component):
                    indexTuple = new IndexTuple((int)Types.Component, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out ecsIem) == 1;
                case nameof(Types.ComponentMember):
                    indexTuple = new IndexTuple((int)Types.ComponentMember, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out ecsIem) == 1;
                case nameof(Types.System):
                    indexTuple = new IndexTuple((int)Types.System, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out ecsIem) == 1;
                case nameof(Types.Entity):
                    indexTuple = new IndexTuple((int)Types.Entity, inputParameter);
                    return projectStructure.DOCListGetItem(indexTuple, out ecsIem) == 1;
                default:
                    indexTuple = new IndexTuple();
                    return false;
            }
        }


        #endregion

        #region Preparation

        private static void SetVisualStudioInstance()
        {
            if (MSBuildLocator.IsRegistered)
            {
                Logger.Separator();
                return;
            }

            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances.Length == 1
                // If there is only one instance of MSBuild on this machine, set that as the one to use.
                ? visualStudioInstances[0]
                // Handle selecting the version of MSBuild you want to use.
                : SelectVisualStudioInstance(visualStudioInstances);

            Logger.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            // NOTE: Be sure to register an instance with the MSBuildLocator 
            //       before calling MSBuildWorkspace.Create()
            //       otherwise, MSBuildWorkspace won't MEF compose.
            MSBuildLocator.RegisterInstance(instance);
            Logger.Separator();
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
        {
            if (Constant.PREFERRED_VSI > 0 && Constant.PREFERRED_VSI <= visualStudioInstances.Length)
            {
                int version = Constant.PREFERRED_VSI - 1;

                Logger.WriteLine("Multiple installs of MSBuild detected, used predefined version:");
                Logger.WriteLine($"    Name: {visualStudioInstances[version].Name}");
                Logger.WriteLine($"    Version: {visualStudioInstances[version].Version}");
                Logger.WriteLine($"    MSBuild Path: {visualStudioInstances[version].MSBuildPath}");

                return visualStudioInstances[version];
            }

            Logger.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++)
            {
                Logger.WriteLine($"Instance {i + 1}");
                Logger.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Logger.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Logger.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true)
            {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length)
                {
                    return visualStudioInstances[instanceNumber - 1];
                }
                Logger.WriteLine("Input not accepted, try again.");
            }
        }


        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Logger.WriteLineToConsoleOnly($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }

        #endregion

        #region Compile and Parse Project

        private static async Task OpenProject()
        {
            workspace = MSBuildWorkspace.Create();

            // Print message for WorkspaceFailed event to help diagnosing project load failures.
            workspace.WorkspaceFailed += (o, e) => Logger.WriteLineDebug(e.Diagnostic.Message);

            Logger.WriteLine($"Loading project '{projectPath}'");

            // Attach progress reporter so we print projects as they are loaded.
            project = await workspace.OpenProjectAsync(projectPath);
            Logger.WriteLine($"Finished loading project '{projectPath}'");
            Logger.Separator();
        }


        private static async Task CompileProject()
        {
            compilation = await project.GetCompilationAsync();
            Logger.WriteLine($"Found syntax trees: {compilation.SyntaxTrees.Count()}");
            Logger.WriteLine("Compilation finished");
            Logger.Separator();
        }

        private static void ParseProject()
        {
            Logger.WriteLine($"Parsing project...");
            TreeWalker.ParseProject(projectStructure, compilation);
            Logger.WriteLine("\nFinished Parsing Project");
            Logger.Separator();
        }

        private static void LinkProject()
        {
            Logger.WriteLine($"Linking project...");
            TreeWalker.LinkProject(projectStructure);
            Logger.WriteLine("Finished Linking Project");
            Logger.Separator();
        }

        private static void ConnectProject()
        {
            Logger.WriteLine($"Connecting project...");
            TreeWalker.ConnectProject(projectStructure);
            Logger.WriteLine("Finished Connecting Project");
            Logger.Separator();
        }

        public static void PlanProject()
        {
            menuForm.UpdateStepLabel($"OOD to ECS Analysis...");

            menuForm.UpdateTotalProgressBar(20);
            Logger.WriteLine($"Planning project...");

            //menuForm.UpdateStepLabel("Gathering Component Fields...");
            TreeWalker.GatherComponentFields(projectStructure);

            //menuForm.UpdateStepLabel("Gathering Systems...");
            TreeWalker.GatherSystems(projectStructure);


            menuForm.UpdateStepLabel("ECS Design Generation...");
            menuForm.UpdateTotalProgressBar(30);
            TreeWalker.CombiningComponentMembers(projectStructure);

            //menuForm.UpdateStepLabel("Collecting Entity Types...");
            TreeWalker.ConnectEntityTypes(projectStructure);

            if (planProfile.OrderVariableTypes)
            {
                menuForm.UpdateStepLabel("Sorting Components based on Field Types...");
                TreeWalker.SortComponentMembersInComponents(projectStructure);
            }
            Logger.WriteLine("\nFinished Planning Project");
            Logger.Separator();

            menuForm.UpdateStepLabel("Done");
            menuForm.UpdateTotalProgressBar(40);
            menuForm.UpdateStepProgressBar(100);
        }

        #endregion

        #region Print

        private static void DebugAll(string inputType)
        {
            string type = inputType.First().ToString().ToUpper() + inputType.Substring(1);

            switch (type)
            {
                case nameof(Types.Namespace): 
                    for (int j = 0; j < projectStructure.oodNamespaces.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.oodNamespaces[j].GetLabel());
                    }
                    break;
                case nameof(Types.Class):
                    for (int j = 0; j < projectStructure.oodClasses.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.oodClasses[j].GetLabel());
                    }
                    break;
                case nameof(Types.Method):
                    for (int j = 0; j < projectStructure.oodMethods.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.oodMethods[j].GetLabel());
                    }
                    break;
                case nameof(Types.ClassGroup):
                    for (int j = 0; j < projectStructure.classGroups.Count; j++)
                    {
                        OODItem oodParent = projectStructure.oodClasses[projectStructure.classGroups[j][0].itemIndex];
                        string groupLabel = $"[{Types.ClassGroup}] {oodParent.name}";

                        Logger.WriteLine((j + 1) + ". " + groupLabel);
                    }
                    break;
                case nameof(Types.MethodGroup):
                    for (int j = 0; j < projectStructure.methodGroups.Count; j++)
                    {
                        OODItem oodParent = projectStructure.oodMethods[projectStructure.methodGroups[j][0].itemIndex];
                        string groupLabel = $"[{Types.MethodGroup}] {oodParent.name}";

                        Logger.WriteLine((j + 1) + ". " + groupLabel);
                    }
                    break;
                case nameof(Types.Component):
                    for (int j = 0; j < projectStructure.ecsComponents.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.ecsComponents[j].GetLabel());
                    }
                    break;
                case nameof(Types.System):
                    for (int j = 0; j < projectStructure.ecsSystems.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.ecsSystems[j].GetLabel());
                    }
                    break;
                case nameof(Types.Entity):
                    for (int j = 0; j < projectStructure.ecsEntityTypes.Count; j++)
                    {
                        Logger.WriteLine((j + 1) + ". " + projectStructure.ecsEntityTypes[j].GetLabel());
                    }
                    break;
                default:
                    Logger.WriteLine("Something went wrong with printing");
                    return;
            }
        }

        private static void DebugSubStructure(IndexTuple indexTuple)
        {

            if (indexTuple.arrayIndex == (int)Types.Namespace || indexTuple.arrayIndex == (int)Types.Class)
            {
                //TreeWalker.PrintDOCFileStructure(projectStructure, indexTuple, new IndexTuple(-1, -1), 0);

                projectStructure.DOCListGetItem(indexTuple, out OODItem oodItem);
                TreeWalker.DebugItem(oodItem, 0);
            }
            else if (indexTuple.arrayIndex == (int)Types.ClassGroup || indexTuple.arrayIndex == (int)Types.MethodGroup)
            {
                TreeWalker.PrintGroup(projectStructure, indexTuple, 0);
            }
            else if (indexTuple.arrayIndex == (int)Types.Component)
            {
                TreeWalker.PrintComponent(projectStructure, indexTuple, 0);
            }

        }


        private static void PrintSubStructure(IndexTuple indexTuple)
        {

            if (indexTuple.arrayIndex == (int)Types.Namespace || indexTuple.arrayIndex == (int)Types.Class)
            {
                TreeWalker.PrintDocItem(projectStructure, indexTuple, new IndexTuple(-1, -1), 0);

                //projectStructure.DOCListGetItem(indexTuple, out DOCItem oodItem);
                //TreeWalker.DebugItem(oodItem, 0);
            }
            else if (indexTuple.arrayIndex == (int)Types.ClassGroup || indexTuple.arrayIndex == (int)Types.MethodGroup)
            {
                TreeWalker.PrintGroup(projectStructure, indexTuple, 0);
            }
            else if (indexTuple.arrayIndex == (int)Types.Component)
            {
                TreeWalker.PrintComponent(projectStructure, indexTuple, 0);
            }
            else if (indexTuple.arrayIndex == (int)Types.System)
            {
                TreeWalker.PrintSystem(projectStructure, indexTuple, 0);
            }

        }

        //private static void PrintTree(int treeNumber, bool printRaw)
        //{
        //    SyntaxTree tree = compilation.SyntaxTrees.ElementAt(treeNumber);
        //
        //    Logger.WriteLine(TreeWalker.FindClassName(tree));
        //
        //    if (printRaw)
        //        TreeWalker.PrintRawItem(projectStructure.oodFiles[treeNumber], 0);
        //    else
        //        TreeWalker.PrintDOCFileStructure(projectStructure, new IndexTuple((int)ArrayIndex.DOCFile, treeNumber), new IndexTuple(-1, -1), 0);
        //}

        #endregion
    }
}