using System;
using Microsoft.Bot.Builder.FormFlow;

namespace CellarIQ.Bot
{
    [Serializable]
    public class WineDetailsForm
    {
        [Prompt("What is the name of the vintner? {||}", AllowDefault = BoolDefault.True)]
        [Describe("Vintner, example: Beckment")]
        public string VintnerName { get; set; }

        [Prompt("What is the name of the vintner? {||}", AllowDefault = BoolDefault.True)]
        [Describe("WineLabel, example: Beckment")]
        public string LabelName { get; set; }

        public static IForm<WineDetailsForm> BuildForm()
        {
            return new FormBuilder<WineDetailsForm>()
                .Build();
        }
    }
}