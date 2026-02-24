using System;
using System.Collections.Generic;

[Serializable]
public sealed class StepperProgressSlideStepModel
{
    public string Title;
    public string Body;

    public StepperProgressSlideStepModel Clone()
    {
        return new StepperProgressSlideStepModel
        {
            Title = Title,
            Body = Body
        };
    }
}

[Serializable]
public sealed class StepperProgressSlideEffectModel
{
    public string HeaderTitle = "Stepper / Multi-stage Form";
    public List<StepperProgressSlideStepModel> Steps = new List<StepperProgressSlideStepModel>();

    public StepperProgressSlideEffectModel Clone()
    {
        var clone = new StepperProgressSlideEffectModel
        {
            HeaderTitle = HeaderTitle,
            Steps = new List<StepperProgressSlideStepModel>()
        };

        if (Steps != null)
        {
            for (var i = 0; i < Steps.Count; i++)
            {
                clone.Steps.Add(Steps[i] != null ? Steps[i].Clone() : null);
            }
        }

        return clone;
    }

    public static StepperProgressSlideEffectModel CreateDefault()
    {
        return new StepperProgressSlideEffectModel
        {
            Steps = new List<StepperProgressSlideStepModel>
            {
                new StepperProgressSlideStepModel
                {
                    Title = "Identity",
                    Body = "Start with your account details and choose a secure sign-in method."
                },
                new StepperProgressSlideStepModel
                {
                    Title = "Profile",
                    Body = "Add public profile metadata and a short intro for collaborators."
                },
                new StepperProgressSlideStepModel
                {
                    Title = "Preferences",
                    Body = "Tune notifications, update cadence, and workspace defaults."
                },
                new StepperProgressSlideStepModel
                {
                    Title = "Review",
                    Body = "Confirm summary, then publish settings to all linked projects."
                }
            }
        };
    }
}
