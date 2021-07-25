using System.Collections.Generic;

namespace ENCODE.Base
{
    public struct QueryResult
    {
        public IndexTuple indexTuple;
        public string name;
        public List<string> value;
        public bool enabled;

        public QueryResult(IndexTuple _indexTuple, string _name, List<string> _value, bool _enabled)
        {
            indexTuple = _indexTuple;
            name = _name;
            value = _value;
            enabled = _enabled;
        }
    }

    static partial class TreeWalker
    {
        public static List<List<QueryResult>> DrawComponent(IndexTuple indexTuple, Project project)
        {
            bool enabled = true;
            QueryResult header = new QueryResult(new IndexTuple(), "", new List<string>() { "" }, enabled);

            // gather component
            ECSComponent ecsComponent = project.ecsComponents[indexTuple.itemIndex];
            List<string> value = new List<string>(); ;

            foreach (IndexTuple fieldIndex in ecsComponent.ecsComponentFields)
            {
                value.Add(project.ecsComponentFields[fieldIndex.itemIndex].GetLabel());
            }
            QueryResult component = new QueryResult(indexTuple, $"Component {indexTuple.itemIndex}", value, enabled);

            return new List<List<QueryResult>>() { new List<QueryResult>() { header, component } };

        }

        public static List<List<QueryResult>> DrawEntity(IndexTuple indexTuple, Project project)
        {
            List<List<QueryResult>> columnRow = new List<List<QueryResult>>();

            List<QueryResult> column = new List<QueryResult>();
            ECSEntityType entity = project.ecsEntityTypes[indexTuple.itemIndex];
            bool enabled = true;

            // Get Entity name
            column.Add(new QueryResult(indexTuple, $"Entity {indexTuple.itemIndex}: {entity.variableName}", new List<string>(), enabled));

            // gather components
            foreach (IndexTuple componentIndex in entity.ecsComponents)
            {
                ECSComponent component = project.ecsComponents[componentIndex.itemIndex];
                List<string> value = new List<string>(); ;

                foreach (IndexTuple fieldIndex in component.ecsComponentFields)
                {
                    value.Add(project.ecsComponentFields[fieldIndex.itemIndex].GetLabel());
                }
                column.Add(new QueryResult(componentIndex, $"Component {componentIndex.itemIndex}", value, enabled));

            }

            columnRow.Add(column);
            return columnRow;

        }

        public static List<List<QueryResult>> DrawEntityGrid(IndexTuple indexTuple, Project project)
        {
            List<List<QueryResult>> columnRow = new List<List<QueryResult>>();

            List<QueryResult> column = new List<QueryResult>();
            ECSEntityType entity = project.ecsEntityTypes[indexTuple.itemIndex];
            bool enabled = true;

            // Get Entity name
            column.Add(new QueryResult(indexTuple, $"Entity {indexTuple.itemIndex}: {entity.variableName}", new List<string>(), enabled));

            // gather components
            for (int compIndex = 0; compIndex < project.ecsComponents.Count; compIndex++)
            {
                IndexTuple componentIndex = new IndexTuple((int)Types.Component, compIndex);
                List<string> value = new List<string>();

                enabled = entity.ecsComponents.Contains(componentIndex);

                ECSComponent component = project.ecsComponents[componentIndex.itemIndex];
                foreach (IndexTuple fieldIndex in component.ecsComponentFields)
                {
                    value.Add(project.ecsComponentFields[fieldIndex.itemIndex].GetLabel());
                }
                column.Add(new QueryResult(componentIndex, $"Component {componentIndex.itemIndex}", value, enabled));
            }


            columnRow.Add(column);
            return columnRow;

            // gather systems
        }


        public static List<List<QueryResult>> DrawSystem(IndexTuple indexTuple, Project project)
        {
            List<List<QueryResult>> columnRow = new List<List<QueryResult>>();

            List<QueryResult> column = new List<QueryResult>();
            ECSSystem system = project.ecsSystems[indexTuple.itemIndex];
            bool enabled = true;

            // Get Entity name
            column.Add(new QueryResult(indexTuple, $"System {indexTuple.itemIndex}: {system.variableName}", new List<string>(), enabled));

            // read components
            List<string> readValue = new List<string>();
            foreach (IndexTuple componentIndex in system.ecsReadComponents)
            {
                ECSComponent component = project.ecsComponents[componentIndex.itemIndex];
                readValue.Add($"Component {componentIndex.itemIndex} ({component.ecsComponentFields.Count} Variables)");
            }
            column.Add(new QueryResult(indexTuple, $"Read Components", readValue, enabled));


            // write components
            List<string> writeValue = new List<string>();
            foreach (IndexTuple componentIndex in system.ecsWriteComponents)
            {
                ECSComponent component = project.ecsComponents[componentIndex.itemIndex];
                writeValue.Add($"Component {componentIndex.itemIndex} ({component.ecsComponentFields.Count} Variables)");
            }
            column.Add(new QueryResult(indexTuple, $"Write Components", writeValue, enabled));

            columnRow.Add(column);
            return columnRow;
        }

        public static List<List<QueryResult>> DrawClass(IndexTuple indexTuple, Project project)
        {
            List<List<QueryResult>> columnRow = new List<List<QueryResult>>();

            List<QueryResult> column = new List<QueryResult>();
            OODClass oodClass = project.oodClasses[indexTuple.itemIndex];
            bool enabled = true;

            // Get Entity name
            column.Add(new QueryResult(indexTuple, $"Class {indexTuple.itemIndex}: {oodClass.name}", new List<string>(), enabled));


            List<string> variableValue = new List<string>(); ;
            // gather variabels
            foreach (IndexTuple variableIndex in oodClass.oodVariables)
            {
                variableValue.Add(project.oodMembers[variableIndex.itemIndex].GetLabel());
            }
            column.Add(new QueryResult(indexTuple, $"Variables", variableValue, enabled));



            // Methods
            foreach (IndexTuple methodIndex in oodClass.oodMethods)
            {
                OODMethod oodMethod = project.oodMethods[methodIndex.itemIndex];
                List<string> value = new List<string>(); ;

                foreach (IndexTuple fieldIndex in oodMethod.oodWriteGlobalVariables)
                {
                    value.Add("+ " + project.oodMembers[fieldIndex.itemIndex].GetLabel());
                }

                foreach (IndexTuple fieldIndex in oodMethod.oodReadGlobalVariables)
                {
                    value.Add("- " + project.oodMembers[fieldIndex.itemIndex].GetLabel());
                }
                column.Add(new QueryResult(methodIndex, $"Method {methodIndex.itemIndex} : {oodMethod.GetLabel()}", value, enabled));

            }


            columnRow.Add(column);
            return columnRow;

            // gather systems
        }

        public static List<List<QueryResult>> DrawMethod(IndexTuple indexTuple, Project project)
        {
            List<List<QueryResult>> columnRow = new List<List<QueryResult>>();

            List<QueryResult> column = new List<QueryResult>();
            OODMethod oodMethod = project.oodMethods[indexTuple.itemIndex];
            bool enabled = true;

            // Get Method name
            column.Add(new QueryResult(indexTuple, $"Method {indexTuple.itemIndex} : {oodMethod.GetLabel()}", new List<string>(), enabled));

            // write variabeles
            List<string> readValue = new List<string>();
            foreach (IndexTuple fieldIndex in oodMethod.oodReadGlobalVariables)
            {
                readValue.Add(project.oodMembers[fieldIndex.itemIndex].GetLabel());
            }
            column.Add(new QueryResult(indexTuple, $"Read Variables", readValue, enabled));

            // Write Variables
            List<string> writeValue = new List<string>();
            foreach (IndexTuple fieldIndex in oodMethod.oodWriteGlobalVariables)
            {
                writeValue.Add(project.oodMembers[fieldIndex.itemIndex].GetLabel());
            }
            column.Add(new QueryResult(indexTuple, $"Write Variables", writeValue, enabled));


            columnRow.Add(column);
            return columnRow;
        }


    }
}
