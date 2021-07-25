using System;

namespace ENCODE.Base
{
    class SparseSetsPlanProfile : PlanProfile
    {
        public SparseSetsPlanProfile() : base()
        {
            profileName = "Sparse Sets Profile";

            InitPerformanceOptions();

            specificPlanningRules = new string[] { Constant.SPECIFIC_RULE_2 };
        }


        private void InitPerformanceOptions()
        {
            performanceSettingsOptions = new PerformanceSettings[performanceSteps];

            // mergeOnClassEqualityLevel, mergeOnSystemEqualityLevel, splitOnReadWriteAccess, splitOnBlittable, orderVariableTypes
            performanceSettingsOptions[0] = new PerformanceSettings(0.5f, 0.5f, true, true, true); // Completely Performant
            performanceSettingsOptions[1] = new PerformanceSettings(1.0f, 0.5f, true, true, true); // Balanced Performance and Maintainability
            performanceSettingsOptions[2] = new PerformanceSettings(1.0f, 0.5f, false, false, true); // Completely Maintainable
        }
    }
}
