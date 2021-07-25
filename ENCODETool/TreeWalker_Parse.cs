using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ENCODE.Base
{
    static partial class TreeWalker
    {
        public static void ParseProject(Project project, Compilation compilation)
        {
            int totalTrees = compilation.SyntaxTrees.Count();
            int writeDevision = (totalTrees / 20) + 1;

            for (int treeIndex = 0; treeIndex < totalTrees; treeIndex++)
            {
                project = ParseTree(compilation, project, treeIndex);

                // Update the progress bar in the window
                int percentage = (int)(treeIndex * 33.3333f / totalTrees);
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (treeIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Parsing Tree {treeIndex}/{totalTrees} - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Parsing Tree {treeIndex}/{totalTrees} - {percentage}% ");
                }

            }
            Logger.ReWrite($"Finished Parsing {totalTrees}/{totalTrees} - 100% ");
        }

        private static Project ParseTree(Compilation compilation, Project projectStructure, int treeNumber)
        {
            SyntaxTree tree = compilation.SyntaxTrees.ElementAt(treeNumber);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            projectStructure.semanticModels.Add(compilation.GetSemanticModel(tree));
            OODFile oodFile = ParseNode(root, projectStructure, treeNumber);
            oodFile.name = projectStructure.oodRawItems.Count().ToString();

            projectStructure.oodRawItems.Add(oodFile);

            return projectStructure;
        }

        private static OODItem ParseNode(SyntaxNode node, Project project, int rootTreeNumber)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ParseNode((NamespaceDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.ClassDeclaration:
                    return ParseNode((ClassDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.StructDeclaration:
                    return ParseNode((StructDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.EnumDeclaration:
                    return ParseNode((EnumDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.MethodDeclaration:
                    return ParseNode((MethodDeclarationSyntax)node, project, rootTreeNumber);
                // #region Expressions
                case SyntaxKind.ExpressionStatement:
                    return ParseNode((ExpressionStatementSyntax)node, project, rootTreeNumber);
                case SyntaxKind.LocalDeclarationStatement:
                    return ParseNode((LocalDeclarationStatementSyntax)node, project, rootTreeNumber);
                case SyntaxKind.IfStatement:
                    return ParseNode((IfStatementSyntax)node, project, rootTreeNumber);
                case SyntaxKind.ReturnStatement:
                    return ParseNode((ReturnStatementSyntax)node, project, rootTreeNumber);
                case SyntaxKind.ForStatement:
                    return ParseNode((ForStatementSyntax)node, project, rootTreeNumber);
                case SyntaxKind.ForEachStatement:
                    return ParseNode((ForEachStatementSyntax)node, project, rootTreeNumber);
                // #region Identifiers and Special Variables
                case SyntaxKind.UsingDirective:
                    return ParseNode((UsingDirectiveSyntax)node, project, rootTreeNumber);
                case SyntaxKind.FieldDeclaration:
                    return ParseNode((FieldDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.PropertyDeclaration:
                    return ParseNode((PropertyDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.VariableDeclaration:
                    return ParseNode((VariableDeclarationSyntax)node, project, rootTreeNumber);
                case SyntaxKind.VariableDeclarator:
                    return ParseNode((VariableDeclaratorSyntax)node, project, rootTreeNumber);
                case SyntaxKind.IdentifierName:
                    return ParseNode((IdentifierNameSyntax)node, project, rootTreeNumber);
                case SyntaxKind.QualifiedName:
                    return ParseNode((QualifiedNameSyntax)node, project, rootTreeNumber);
                case SyntaxKind.GenericName:
                    return ParseNode((GenericNameSyntax)node, project, rootTreeNumber);
                // Leftovers
                default:
                    return ParseLeftoverNode(node, project, rootTreeNumber);
            }

        }

        private static OODFile ParseNode(CompilationUnitSyntax node, Project project, int rootTreeNumber)
        {
            OODFile oodItem = new OODFile("", rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }

            return oodItem;
        }

        private static OODNamespace ParseNode(NamespaceDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            var symbol = project.semanticModels[rootTreeNumber].GetDeclaredSymbol(node);
            string name = !symbol.ContainingNamespace.IsGlobalNamespace ? $"{symbol.ContainingNamespace}.{symbol.Name}" : symbol.Name;

            OODNamespace oodItem = new OODNamespace(name, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODClass ParseNode(ClassDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            var symbol = project.semanticModels[rootTreeNumber].GetDeclaredSymbol(node);
            OODClass oodItem = new OODClass(symbol.Name, symbol, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODClass ParseNode(StructDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            var symbol = project.semanticModels[rootTreeNumber].GetDeclaredSymbol(node);
            OODClass oodItem = new OODClass(symbol.Name, symbol, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODMethod ParseNode(MethodDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            var symbol = project.semanticModels[rootTreeNumber].GetDeclaredSymbol(node);
            OODMethod oodItem = new OODMethod(symbol.Name, symbol, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODMember ParseLeftoverNode(SyntaxNode node, Project project, int rootTreeNumber)
        {
            OODMember oodItem = new OODMember("", node.Kind().ToString(), rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        public static string GetIdentifier(SyntaxNode node, string output)
        {

            if (node.Kind() == SyntaxKind.IdentifierName)
            {
                if (output != "")
                    output += ".";
                output += ((IdentifierNameSyntax)node).Identifier.ToString();
            }
            else
            {
                foreach (var childNode in node.ChildNodes())
                {
                    output = GetIdentifier(childNode, output);
                }
            }
            return output;
        }


        #region Expressions
        private static OODExpression ParseNode(ExpressionStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Expression.ToString();

            OODExpression oodItem = new OODExpression(name, Constant.EXPRESSION_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODExpression ParseNode(LocalDeclarationStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Declaration.ToString();

            OODExpression oodItem = new OODExpression(name, Constant.LOCAL_DECLARATION_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODExpression ParseNode(ReturnStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.ToString();

            OODExpression oodItem = new OODExpression(name, Constant.RETURN_EXPRESSION_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODExpression ParseNode(IfStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Condition.ToString();

            OODExpression oodItem = new OODExpression(name, Constant.IF_EXPRESSION_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODExpression ParseNode(ForStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = $"{node.Declaration}; {node.Condition}; {node.Incrementors};";

            OODExpression oodItem = new OODExpression(name, Constant.FOR_STATEMENT_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODExpression ParseNode(ForEachStatementSyntax node, Project project, int rootTreeNumber)
        {
            string name = $"{node.Type} {node.Identifier} in {node.Expression};";

            OODExpression oodItem = new OODExpression(name, Constant.FOREACH_STATEMENT_TYPE, rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                if (childItem.GetType() == typeof(OODMember) && childItem.name == node.Type.ToString())
                {
                    childItem.name = node.Identifier.ToString();
                    ((OODMember)childItem).variableType = node.Type.ToString();
                }
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        #endregion

        #region Identifiers and Special Variables

        private static OODMember ParseNode(UsingDirectiveSyntax node, Project project, int rootTreeNumber)
        {
            if (node.Alias != null || node.StaticKeyword.ToString() == "static")
                return ParseLeftoverNode(node, project, rootTreeNumber);

            string name = GetIdentifier(node, "");

            // Create an Ignore List
            string ignoreList = CreateIgnoreList(node, name, project, rootTreeNumber);
            OODMember oodItem = new OODMember(name, Constant.USING_TYPE, rootTreeNumber) { variableType = ignoreList };

            return oodItem;
        }

        private static string CreateIgnoreList(UsingDirectiveSyntax node, string name, Project project, int rootTreeNumber)
        {

            var systemSymbol = (INamespaceSymbol)project.semanticModels[rootTreeNumber].GetSymbolInfo(node.Name).Symbol;

            var usingMembers = systemSymbol.GetMembers();
            string ignoreList = name;

            // Also add alias (needs some trimming)
            if (node.Alias != null)
                ignoreList += node.Alias.ToString().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

            int skipLength = name.Length + 1;

            foreach (var member in usingMembers)
            {
                int skip = Math.Min(skipLength, member.ToString().Length);
                ignoreList += $",{member.ToString().Substring(skip)}";
            }
            return ignoreList;
        }

        private static OODMember ParseNode(FieldDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Declaration.Variables.LastOrDefault().Identifier.ValueText;
            string variableType = node.Declaration.Type.ToString();
            ISymbol systemSymbol = project.semanticModels[rootTreeNumber].GetSymbolInfo(node).Symbol;

            if (FromImportedPackage(systemSymbol))
                rootTreeNumber = -1;

            OODMember oodItem = new OODMember(name, Constant.GLOBAL_DECLARATION_TYPE, rootTreeNumber) { variableType = variableType };

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODMember ParseNode(PropertyDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Identifier.ToString();
            string variableType = node.Type.ToString();
            ISymbol systemSymbol = project.semanticModels[rootTreeNumber].GetSymbolInfo(node).Symbol;

            if (FromImportedPackage(systemSymbol))
                rootTreeNumber = -1;

            OODMember oodItem = new OODMember(name, Constant.GLOBAL_DECLARATION_TYPE, rootTreeNumber) { variableType = variableType };

            return oodItem;
        }


        private static OODMember ParseNode(EnumDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Identifier.ToString();
            string variableType = "enum";

            OODMember oodItem = new OODMember(name, Constant.ENUM_DECLARATION_TYPE) { variableType = variableType };

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }


        private static OODMember ParseNode(VariableDeclarationSyntax node, Project project, int rootTreeNumber)
        {
            string variableType = node.Type.ToString();
            ISymbol systemSymbol = project.semanticModels[rootTreeNumber].GetSymbolInfo(node).Symbol;

            if (FromImportedPackage(systemSymbol))
                rootTreeNumber = -1;

            OODMember oodItem = new OODMember("", node.Kind().ToString(), rootTreeNumber) { variableType = variableType };

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }

        private static OODMember ParseNode(VariableDeclaratorSyntax node, Project project, int rootTreeNumber)
        {
            var symbol = project.semanticModels[rootTreeNumber].GetDeclaredSymbol(node);
            string name = node.Identifier.ToString();
            ISymbol systemSymbol = project.semanticModels[rootTreeNumber].GetSymbolInfo(node).Symbol;

            if (FromImportedPackage(systemSymbol))
                rootTreeNumber = -1;

            OODMember oodItem = new OODMember(name, node.Kind().ToString(), rootTreeNumber);

            foreach (var childNode in node.ChildNodes())
            {
                OODItem childItem = ParseNode(childNode, project, rootTreeNumber);
                oodItem.oodRawItems.Add(childItem);
            }
            return oodItem;
        }


        private static OODMember ParseNode(IdentifierNameSyntax node, Project project, int rootTreeNumber)
        {
            string name = node.Identifier.ToString();
            ISymbol systemSymbol = project.semanticModels[rootTreeNumber].GetSymbolInfo(node).Symbol;

            if (FromImportedPackage(systemSymbol))
                rootTreeNumber = -1;

            OODMember oodItem = new OODMember(name, Constant.IDENTIFIER_TYPE, rootTreeNumber);

            return oodItem;
        }

        private static bool FromImportedPackage(ISymbol systemSymbol)
        {
            return systemSymbol != null && systemSymbol.GetType().Name != "PropertySymbol" && systemSymbol.GetType().Name != "MethodSymbol" &&
                systemSymbol.ContainingAssembly != null && systemSymbol.ContainingAssembly.Name != "Assembly-CSharp";
        }

        private static OODMember ParseNode(QualifiedNameSyntax node, Project project, int rootTreeNumber)
        {
            string name = $"{node.Left}.{node.Right}";

            OODMember oodItem = new OODMember(name, Constant.IDENTIFIER_TYPE, rootTreeNumber);

            return oodItem;
        }

        private static OODMember ParseNode(GenericNameSyntax node, Project project, int rootTreeNumber)
        {
            string name = $"{node.Identifier}{node.TypeArgumentList}";

            OODMember oodItem = new OODMember(name, Constant.IDENTIFIER_TYPE, rootTreeNumber);

            return oodItem;
        }


        #endregion


    }
}