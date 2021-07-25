using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENCODE.Base
{

    public static class Constant
    {
        public const string LOG_FILE_PATH = @"DOCLogFile.txt";

        public const int PREFERRED_VSI = 1;
        public const bool DEBUG_OUTPUT = false;

        // All recognized blittable types
        public static readonly string[] BLITTABLE_TYPES = { "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "float2", "float3", "double" };
        public static readonly string[] NON_BLITTABLE_TYPES = { "bool", "string", "char", "Text" };


        // -- Don't change anything below unless you know what you are doing -- //
        public const string PROJECT_TYPE = "Project";
        public const string USING_TYPE = "Using";
        public const string IDENTIFIER_TYPE = "Identifier";

        public const string EXPRESSION_TYPE = "Expression";
        public const string RETURN_EXPRESSION_TYPE = "ReturnExpression";
        public const string IF_EXPRESSION_TYPE = "IfExpression";
        public const string LOCAL_DECLARATION_TYPE = "LocalDeclaration";
        public const string FOR_STATEMENT_TYPE = "ForStatement";
        public const string FOREACH_STATEMENT_TYPE = "ForeachStatement";

        public const string GLOBAL_DECLARATION_TYPE = "GlobalDeclaration";
        public const string ENUM_DECLARATION_TYPE = "Enum";


        public static readonly string[] MENU_OUTPUT_MODE = { "debug", "draw", "print" };
        public static readonly string[] MENU_INPUT_PARAM = { "all" };
        public static readonly string[] MENU_INPUT_TYPE =  {
            Types.Namespace.ToString().ToLower(),
            Types.Class.ToString().ToLower(),
            Types.Method.ToString().ToLower(),
            Types.ClassGroup.ToString().ToLower(),
            Types.MethodGroup.ToString().ToLower(),
            Types.Component.ToString().ToLower(),
            Types.System.ToString().ToLower()
        };

        // Method group options
        public const int NO_GROUP = 0;
        public const int SUB_COLLAPSED_GROUP = 1;
        public const int SUB_EXPANDED_GROUP = 2;
        public const int NEW_COLLAPSED_GROUP = 3;
        public const int NEW_EXPANDED_GROUP = 4;
        public const int ADD_TO_GROUP = 5;


        public static string GENERAL_RULE_1 = "Consider putting data that is not used together in separate components, for better cache usage.";
        public static string GENERAL_RULE_2 = "Always split a component if one system writes to it and another system needs to read from/write to it simultaneously, for less dependencies between systems.";
        public static string GENERAL_RULE_3 = "Consider splitting components on domain boundaries, for better understanding, maintainability and test-ability.";
        public static string GENERAL_RULE_4 = "Consider combining data that is processed together, in a single component to improve readability.";
        public static string GENERAL_RULE_5 = "Consider combining data that belongs to the same domain in a single component to improve readability.";
        public static string GENERAL_RULE_6 = "Consider merging components if they do not need to be performant, to make it user friendlier.";
        public static string GENERAL_RULE_7 = "Consider generalizing very specifically used or named components.";
        public static string GENERAL_RULE_8 = "If a system processes a lot of components, consider splitting up your system, as it can be a super system, which is an ECS anti-pattern.";
        public static string GENERAL_RULE_9 = "Always order the data by their data type and size, to avoid unnecessary padding.";

        public static readonly string[] GENERAL_RULES = { GENERAL_RULE_1, GENERAL_RULE_2, GENERAL_RULE_3, GENERAL_RULE_4, GENERAL_RULE_5, GENERAL_RULE_6, GENERAL_RULE_7, GENERAL_RULE_8, GENERAL_RULE_9 };


        public static string SPECIFIC_RULE_1 = "(Archetype-based) Consider minimizing structural changes, for better performance.";
        public static string SPECIFIC_RULE_2 = "(Sparse Sets-based) Consider creating fewer, larger components, to improve entity query performance.";
        public static string SPECIFIC_RULE_3 = "(Unity) Always put reference data in a separate component from blittable data.";
        public static string SPECIFIC_RULE_4 = "(Unity) Consider creating single-field components, to allow vectorization.";

    }


    public enum Types
    {
        File,
        Namespace,
        Class,
        Method,

        Entity,
        Component,
        System,

        Member,
        Expression,

        ClassGroup,
        MethodGroup,

        ComponentMember,
    }
}
