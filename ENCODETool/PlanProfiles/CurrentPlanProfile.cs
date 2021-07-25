using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    class CurrentPlanProfile : PlanProfile
    {
        List<PlanProfile> availablePlanProfiles;
        int performanceOrientationLevel;

        float mergeOnClassEqualityLevel;
        float mergeOnSystemEqualityLevel;
        bool splitOnReadWriteAccess;
        bool splitOnBlittable;
        bool orderVariableTypes;

        public CurrentPlanProfile() : base()
        {
            availablePlanProfiles = new List<PlanProfile> { new GeneralPlanProfile(), new UnityPlanProfile(), new SparseSetsPlanProfile() };

            // Select the planning profile to modify the generated plan
            ChangeToPlanProfile(availablePlanProfiles[0].ProfileName);
        }

        public void ChangeToPlanProfile(string planProfileName)
        {
            if (planProfileName == "Custom")
            {
                profileName = planProfileName; 
                return;
            }

            foreach (PlanProfile planProfile in availablePlanProfiles)
            {
                if (planProfile.ProfileName == planProfileName)
                {
                    // Change current values to the profiles
                    profileName = planProfile.ProfileName;
                    blittableTypes = planProfile.BlittableTypes;
                    nonBlittableTypes = planProfile.NonBlittableTypes;
                    specificPlanningRules = planProfile.SpecificPlanningRules;
                    knownParentClasses = planProfile.KnownParentClasses;
                    performanceSettingsOptions = planProfile.PerformanceSettingsOptions;
                    ChangePerformanceOrientation(performanceOrientationLevel);
                    return;
                }
            }
        }

        public void ChangePerformanceOrientation(int value)
        {
            performanceOrientationLevel = value;

            orderVariableTypes = performanceSettingsOptions[value].OrderVariableTypes;
            splitOnBlittable = performanceSettingsOptions[value].SplitOnBlittable;
            mergeOnClassEqualityLevel = performanceSettingsOptions[value].MergeOnClassEqualityLevel;
            mergeOnSystemEqualityLevel = performanceSettingsOptions[value].MergeOnSystemEqualityLevel;
            splitOnReadWriteAccess = performanceSettingsOptions[value].SplitOnReadWriteAccess;
        }

        #region Getters  and Setters

        public List<PlanProfile> AvailablePlanProfiles
        {
            get { return availablePlanProfiles; }
            set { availablePlanProfiles = value; }
        }

        public int PerformanceOrientationLevel
        {
            get { return performanceOrientationLevel; }
            set { performanceOrientationLevel = value; }
        }


        public float MergeOnClassEqualityLevel
        {
            get { return mergeOnClassEqualityLevel; }
            set { mergeOnClassEqualityLevel = value; }
        }

        public float MergeOnSystemEqualityLevel
        {
            get { return mergeOnSystemEqualityLevel; }
            set { mergeOnSystemEqualityLevel = value; }
        }

        public bool SplitOnReadWriteAccess
        {
            get { return splitOnReadWriteAccess; }
            set { splitOnReadWriteAccess = value; }
        }

        public bool SplitOnBlittable
        {
            get { return splitOnBlittable; }
            set { splitOnBlittable = value; }
        }


        public bool OrderVariableTypes
        {
            get { return orderVariableTypes; }
            set { orderVariableTypes = value; }
        }
        #endregion
    }
}
