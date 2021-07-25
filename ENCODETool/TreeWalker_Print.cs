using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    static partial class TreeWalker
    {

        #region Print Raw

        public static void DebugItem(OODItem oodItem, int depth)
        {
            PrintLine(oodItem.GetLabel(), depth);

            depth++;

            foreach (var item in oodItem.oodRawItems)
            {
                DebugItem(item, depth);
            }
        }

        #endregion

        #region Print Intermediate OOD Model

        public static void PrintDocItem(Project project, IndexTuple indexTuple, IndexTuple parentIndexTuple, int depth)
        {
            int parentIndex = project.ListGetItem(indexTuple, parentIndexTuple, out OODItem oodItem);
            if (oodItem == null || (oodItem.type == Types.File.ToString() && parentIndex == -1))
                return;

            PrintLine(oodItem.GetLabel(), depth);

            depth++;

            foreach (var childIndexTuple in oodItem.GetAllChildren(-1))
            {
                PrintDocItem(project, childIndexTuple, indexTuple, depth);
            }
        }


        public static void PrintGroup(Project project, IndexTuple indexTuple, int depth)
        {
            if (project.GroupListGetItems(indexTuple, out List<IndexTuple> groupMembers) != 1)
                return;

            project.ListGetItem(groupMembers[0], out OODItem oodParent);
            PrintLine(oodParent.GetLabel(), depth);

            depth++;

            // Add Children Recursively
            for (int i = 1; i < groupMembers.Count; i++)
            {
                project.ListGetItem(groupMembers[i], out OODItem oodChild);
                PrintLine(oodChild.GetLabel(), depth);
            }

        }

        #endregion

        #region Print ECS

        public static void PrintComponent(Project project, IndexTuple indexTuple, int depth)
        {
            if (project.ListGetItem(indexTuple, out ECSItem ecsItem) != 1)
                return;

            ECSComponent ecsComponent = (ECSComponent)ecsItem;
            PrintLine(ecsComponent.GetLabel(), depth);

            depth++;

            // Add Children Recursively
            for (int i = 0; i < ecsComponent.ecsComponentFields.Count; i++)
            {
                project.ListGetItem(ecsComponent.ecsComponentFields[i], out ECSItem oodChild);
                PrintLine(oodChild.GetLabel(), depth);
            }

        }


        #endregion


        private static void PrintLine(string member, int depth)
        {
            string line = "";
            for (int i = 0; i < depth; i++)
                line += "|";

            line += member;

            Logger.WriteLine(line);
        }

    }
}

