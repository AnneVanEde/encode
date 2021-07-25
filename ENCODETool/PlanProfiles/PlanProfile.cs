using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    abstract class PlanProfile
    {
        protected string profileName;

        protected List<string> blittableTypes;
        protected List<string> nonBlittableTypes;
        protected string[] specificPlanningRules;

        /// <summary>
        /// 
        /// </summary>
        protected List<OODClass> knownParentClasses;

        protected PerformanceSettings[] performanceSettingsOptions;
        protected const int performanceSteps = 3;

        public PlanProfile()
        {
            blittableTypes = new List<string>();
            nonBlittableTypes = new List<string>();
            knownParentClasses = new List<OODClass>();
            AddDefaultInformation();
        }


        private void AddDefaultInformation()
        {
            blittableTypes.AddRange(new List<string>() { "Vector2", "Vector3", "GameObject" });

            OODClass monobehaviourClass = new OODClass("MonoBehaviour", null) { namespaceName = "UnityEngine" };
            List<OODMember> monobehaviourFields = new List<OODMember>()
            {
                new OODMember("useGUILayout", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("runInEditMode", Constant.GLOBAL_DECLARATION_TYPE),

                new OODMember("enabled", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("isActiveAndEnabled", Constant.GLOBAL_DECLARATION_TYPE),

                new OODMember("particleSystem", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("transform", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("gameObject", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("tag", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("rigidbody", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("hingeJoint", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("camera", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("rigidbody2D", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("animation", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("constantForce", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("renderer", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("audio", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("networkView", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("collider", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("collider2D", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("light", Constant.GLOBAL_DECLARATION_TYPE),

                new OODMember("hideFlags", Constant.GLOBAL_DECLARATION_TYPE),
                new OODMember("name", Constant.GLOBAL_DECLARATION_TYPE),
            };

            monobehaviourClass.oodRawItems.AddRange(monobehaviourFields);
            knownParentClasses.Add(monobehaviourClass);
        }

        #region Getters  and Setters

        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }

        public List<string> BlittableTypes
        {
            get { return blittableTypes; }
            set { blittableTypes = value; }
        }

        public List<string> NonBlittableTypes
        {
            get { return nonBlittableTypes; }
            set { nonBlittableTypes = value; }
        }

        public string[] SpecificPlanningRules
        {
            get { return specificPlanningRules; }
            set { specificPlanningRules = value; }
        }

        public List<OODClass> KnownParentClasses
        {
            get { return knownParentClasses; }
            set { knownParentClasses = value; }
        }

        public PerformanceSettings[] PerformanceSettingsOptions
        {
            get { return performanceSettingsOptions; }
            set { performanceSettingsOptions = value; }
        }

        public int PerformanceSteps
        {
            get { return performanceSteps; }
        }

        #endregion
    }

    public struct PerformanceSettings
    {
        /// <summary>
        /// Wether or not a components variables should be ordered on type (floats by floats, strings by strings)
        /// </summary>
        bool orderVariableTypes;

        /// <summary>
        /// Wether or not a component should split blittable and non blittable data
        /// </summary>
        bool splitOnBlittable;

        /// <summary>
        /// Variable for how much variables should be grouped on class/entity type
        /// -1     : Every variable in a separate component
        /// [0, 1> : Variables should have most classes in common
        /// 1      : Variables should only be grouped if all of them have all classes/entity types in common
        /// </summary>
        float mergeOnClassEqualityLevel;


        /// <summary>
        /// Variable for how much variables should be grouped on system access
        /// -1     : Every variable in a separate component
        /// [0, 1> : Variables should have most system in common
        /// 1      : Variables should only be grouped if all of them have all system accesses in common
        /// </summary>
        float mergeOnSystemEqualityLevel;

        /// <summary>
        /// Variable for taking read and write separation into account when merging component variables based on systems
        /// true   : (Advised) Seperating read access variables from write access variables if they are used in the same system
        /// false  : Only ensures that the variables are used in the same systems
        /// </summary>
        bool splitOnReadWriteAccess;

        public PerformanceSettings(
            float _mergeOnClassEqualityLevel,
            float _mergeOnSystemEqualityLevel,
            bool _splitOnReadWriteAccess,
            bool _splitOnBlittable,
            bool _orderVariableTypes)
        {
            orderVariableTypes = _orderVariableTypes;
            splitOnBlittable = _splitOnBlittable;
            mergeOnClassEqualityLevel = _mergeOnClassEqualityLevel;
            mergeOnSystemEqualityLevel = _mergeOnSystemEqualityLevel;
            splitOnReadWriteAccess = _splitOnReadWriteAccess;
        }

        #region Getters  and Setters
        public bool SplitOnReadWriteAccess
        {
            get { return splitOnReadWriteAccess; }
        }

        public bool SplitOnBlittable
        {
            get { return splitOnBlittable; }
        }

        public bool OrderVariableTypes
        {
            get { return orderVariableTypes; }
        }

        public float MergeOnClassEqualityLevel
        {
            get { return mergeOnClassEqualityLevel; }
        }

        public float MergeOnSystemEqualityLevel
        {
            get { return mergeOnSystemEqualityLevel; }
        }

        #endregion
    }
}
