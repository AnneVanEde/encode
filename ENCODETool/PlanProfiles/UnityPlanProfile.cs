using System;

namespace ENCODE.Base
{
     class UnityPlanProfile : PlanProfile
    {
        public UnityPlanProfile() : base()
        {
            profileName = "Unity Profile";

            InitPerformanceOptions();

            specificPlanningRules = new string[] { Constant.SPECIFIC_RULE_1, Constant.SPECIFIC_RULE_3, Constant.SPECIFIC_RULE_4 };
        }


        private void InitPerformanceOptions()
        {
            performanceSettingsOptions = new PerformanceSettings[performanceSteps];

            // mergeOnClassEqualityLevel, mergeOnSystemEqualityLevel, splitOnReadWriteAccess, splitOnBlittable, orderVariableTypes
            performanceSettingsOptions[0] = new PerformanceSettings(-1.0f, -1.0f, true, true, true); // Completely Performant
            performanceSettingsOptions[1] = new PerformanceSettings(1.0f, 1.0f, true, true, true); // Balanced Performance and Maintainability
            performanceSettingsOptions[2] = new PerformanceSettings(0.5f, 0.5f, false, false, true); // Completely Maintainable
        }
    }
}
