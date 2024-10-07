using BehaviorTreeForTests.Tests.Common;
using Medbullets.CrossCutting.Extensions;
using Medbullets.Tests.Utils.BehaviorTreeApproach.Core;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests.Samples.Sample
{
    [TestFixture]
    public class MicrowaveOvenTests
    {
        // [Test]
        public async Task All()
        {
            var report = await new BehaviorTree(
                    new Sequence(
                        new MakeDoorClosedWithoutFoodInside(),
                        new CheckR6_TurnOffTheLight(),
                        new Branching(
                            new Sequence(
                                new OpenTheDoor(),
                                new CheckR4_Light_WhenCooking_Or_DoorOpened(),
                                new PushTheButton(),
                                new CheckR3_PushingButtonHasNoEffect(),
                                new CloseTheDoor(),
                                new CheckR6_TurnOffTheLight()
                            ),
                            new Sequence(
                                new PushTheButton(),
                                new CheckR1_StartsCooking(),
                                new CheckR4_Light_WhenCooking_Or_DoorOpened(),
                                new PushTheButton(),
                                new CheckR2_ExtraMinute(),

                                new Branching(
                                    new Sequence(
                                        new OpenTheDoor(),
                                        new CheckR5_StopsWhenOpenTheDoor()
                                    ),
                                    new Sequence(
                                        new WaitForTimeIsOut(),
                                        new CheckR7_TimeIsOut_BeeperAndLight()
                                    )
                                )
                            )
                        )
                    )
                )
                .GetAllScenarios<MicrowaveOvenContext>()
                .ExecuteAll(() => new MicrowaveOvenContext());

            report.WriteToOutput(Console.Write);
        }

        // [Test, Timeout(1000)]
        public async Task FlowN1()
        {
            var c = new MicrowaveOvenContext();
            await new MakeDoorClosedWithoutFoodInside().Execute(c);
            await new CheckR6_TurnOffTheLight().Execute(c);
            await new OpenTheDoor().Execute(c);
            await new CheckR4_Light_WhenCooking_Or_DoorOpened().Execute(c);
            await new PushTheButton().Execute(c);
            await new CheckR3_PushingButtonHasNoEffect().Execute(c);
            await new CloseTheDoor().Execute(c);
            await new CheckR6_TurnOffTheLight().Execute(c);
        }

        public class MicrowaveOvenContext : Proxy<MicrowaveOven>
        {
            protected override MicrowaveOven Create()
            {
                return new MicrowaveOven();
            }
        }

        #region Actions

        public class MakeDoorClosedWithoutFoodInside : IBehavior<MicrowaveOvenContext>
        {
            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Instantiate();
            }
        }

        public class PushTheButton : IBehavior<MicrowaveOvenContext>
        {
            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PushTheButton();
            }
        }

        public class OpenTheDoor : IBehavior<MicrowaveOvenContext>
        {
            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.OpenTheDoor();
            }
        }

        public class CloseTheDoor : IBehavior<MicrowaveOvenContext>
        {
            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.CloseTheDoor();
            }
        }

        public class DoNothing : IBehavior<MicrowaveOvenContext>
        {
            public async Task Execute(MicrowaveOvenContext context)
            {
            }
        }

        public class WaitForTimeIsOut : IBehavior<MicrowaveOvenContext>
        {
            public Task Execute(MicrowaveOvenContext context)
            {
                var completed = new TaskCompletionSource();

                context.Item.IsWorking.Subscribe(
                    x =>
                    {
                        if (x == false)
                        {
                            completed.TrySetResult();
                        }
                    }
                );

                return Task.WhenAny(Task.Delay(1.Seconds()), completed.Task); // check timeout, throw exception
            }
        }

        #endregion

        #region Requirements

        public class CheckR1_StartsCooking : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "If the oven is idle and you push the button, the oven\r\nwill start cooking (that is, energise the power-tube for\r\none minute).";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PowerTube.IsEnergised.ClaimTrue();
                context.Item.Remains.Seconds.ClaimEqual(60);
            }
        }

        public class CheckR2_ExtraMinute : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "If the button is pushed while the oven is cooking it will\r\ncause the oven to cook for an extra minute.";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PowerTube.IsEnergised.ClaimTrue();
                context.Item.Remains.Seconds.ClaimEqual(120);
            }
        }

        public class CheckR3_PushingButtonHasNoEffect : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "Pushing the button when the door is open has no effect\r\n(because it is disabled).";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PowerTube.IsEnergised.ClaimFalse();
            }
        }

        public class CheckR4_Light_WhenCooking_Or_DoorOpened : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "Whenever the oven is cooking or the door is open the\r\nlight in the oven will be on.";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.Light.IsOn.ClaimTrue();
            }
        }

        public class CheckR5_StopsWhenOpenTheDoor : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "Opening the door stops the cooking.";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PowerTube.IsEnergised.ClaimFalse();
            }
        }

        public class CheckR6_TurnOffTheLight : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "Closing the door turns off the light. This is the normal\r\nidle state prior to cooking when the user has placed\r\nfood in the oven.";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.Light.IsOn.ClaimFalse();
            }
        }

        public class CheckR7_TimeIsOut_BeeperAndLight : IBehavior<MicrowaveOvenContext>, IRequirement
        {
            public string Description { get; } = "If the oven times-out, the light and the power-tube are\r\nturned off and then a beeper emits a sound to indicate\r\nthat the cooking is finished.";

            public async Task Execute(MicrowaveOvenContext context)
            {
                context.Item.PowerTube.IsEnergised.ClaimFalse();
                context.Item.Light.IsOn.ClaimFalse();
                context.Item.Beeper.IsActive.ClaimTrue();
            }
        }

        #endregion
    }
}