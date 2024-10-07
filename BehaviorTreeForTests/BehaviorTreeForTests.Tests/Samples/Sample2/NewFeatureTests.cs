using BehaviorTreeForTests.Tests.Common;
using Medbullets.CrossCutting.Extensions;
using Medbullets.Tests.Utils.BehaviorTreeApproach.Core;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests.Samples.Sample2
{
    public class NewFeatureTests
    {
        #region Behavior Tree approach

        [Test]
        public async Task Step1()
        {
            var b = new BehaviorTree(
                new Sequence(
                    new Step(
                        "Tapping on push notification should navigate to feature proposal. " +
                        "Once user applies to it, new tab become available for the user.",
                        async c =>
                        {
                            c.TapOnNotification();
                            c.EnsureAppIsRunning();
                            c.EnsureNavigatedToFeatureProposal();
                            c.Item.FeatureProposal.Apply();
                            c.Item.Feature.IsEnabled.ClaimTrue();
                        }
                    )
                )
            );

            var s = b.GetAllScenarios<Context>();

            var r = await s.ExecuteAll(() => new Context());

            r.WriteToOutput(Console.Write);
            r.FailIfOneOfScenariosFailed();
        }

        [Test]
        public async Task Step2()
        {
            var b = new BehaviorTree(
                new Sequence(
                    new Branching(
                        new Step(
                            "App is started",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            }
                        ).As(out var appIsStarted),

                        new Step(
                            "App is closed",
                            async c =>
                            {
                                c.EnsureAppIsNotRunning();
                            }
                        ).As(out var appIsClosed)
                    ),

                    new Step(
                        "Tap on push notification",
                        async c =>
                        {
                            c.TapOnNotification();
                        }
                    ),

                    new When(appIsClosed,
                        new Step("Ensure app is running now",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            })
                    ),

                    new Step(
                        "Tapping on push notification should navigate to feature proposal. " +
                        "Once user applies to it, new tab become available for the user.",
                        async c =>
                        {
                            c.EnsureNavigatedToFeatureProposal();
                            c.Item.FeatureProposal.Apply();
                            c.Item.Feature.IsEnabled.ClaimTrue();
                        }
                    )
                )
            );

            var s = b.GetAllScenarios<Context>();

            var r = await s.ExecuteAll(() => new Context());

            r.WriteToOutput(Console.Write);
            r.FailIfOneOfScenariosFailed();
        }

        [Test]
        public async Task Step2_UserState()
        {
            var b = new BehaviorTree(
                new Sequence(
                    new Branching(
                        new Step(
                            "User is logged in",
                            async c =>
                            {
                                c.PrepareLoggedInContext();
                            }
                        ).As(out var userIsLoggedIn),

                        new Step(
                            "User is not logged in",
                            async c =>
                            {
                                c.EnsureAppIsNotRunning();
                            }
                        ).As(out var userIsNotLoggedIn)
                    ),

                    new Branching(
                        new Step(
                            "App is started",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            }
                        ).As(out var appIsStarted),

                        new Step(
                            "App is closed",
                            async c =>
                            {
                                c.EnsureAppIsNotRunning();
                            }
                        ).As(out var appIsClosed)
                    ),

                    new Step(
                        "Tap on push notification",
                        async c =>
                        {
                            c.TapOnNotification();
                        }
                    ),

                    new When(appIsClosed,
                        new Step("Ensure app is running now",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            })
                    ),

                    new When(userIsNotLoggedIn,
                        new Step("Log in and continue the flow",
                            async c =>
                            {
                                c.EnsureNavigatedToLogin();
                                c.PerformLogIn();
                                c.EnsureUserIsLoggedIn();
                            })
                    ),

                    new Step(
                        "Tapping on push notification should navigate to feature proposal. " +
                        "Once user applies to it, new tab become available for the user.",
                        async c =>
                        {
                            c.EnsureNavigatedToFeatureProposal();
                            c.Item.FeatureProposal.Apply();
                            c.Item.Feature.IsEnabled.ClaimTrue();
                        }
                    )
                )
            );

            var s = b.GetAllScenarios<Context>();

            var r = await s.ExecuteAll(() => new Context());

            r.WriteToOutput(Console.Write);
            r.FailIfOneOfScenariosFailed();
        }

        [Test]
        public async Task Step3_Reject()
        {
            var b = new BehaviorTree(
                new Sequence(
                    new Branching(
                        new Step(
                            "User is logged in",
                            async c =>
                            {
                                c.PrepareLoggedInContext();
                            }
                        ).As(out var userIsLoggedIn),

                        new Step(
                            "User is not logged in",
                            async c =>
                            {
                                c.EnsureAppIsNotRunning();
                            }
                        ).As(out var userIsNotLoggedIn)
                    ),

                    new Branching(
                        new Step(
                            "App is started",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            }
                        ).As(out var appIsStarted),

                        new Step(
                            "App is closed",
                            async c =>
                            {
                                c.EnsureAppIsNotRunning();
                            }
                        ).As(out var appIsClosed)
                    ),

                    new Step(
                        "Tap on push notification",
                        async c =>
                        {
                            c.TapOnNotification();
                        }
                    ),

                    new When(
                        appIsClosed,
                        new Step(
                            "Ensure app is running now",
                            async c =>
                            {
                                c.EnsureAppIsRunning();
                            }
                        )
                    ),

                    new When(
                        userIsNotLoggedIn,
                        new Step(
                            "Log in and continue the flow",
                            async c =>
                            {
                                c.EnsureNavigatedToLogin();
                                c.PerformLogIn();
                                c.EnsureUserIsLoggedIn();
                            }
                        )
                    ),

                    new Step(
                        "Tapping on push notification should navigate to feature proposal.",
                        async c =>
                        {
                            c.EnsureNavigatedToFeatureProposal();
                        }
                    ),

                    new Branching(
                        new Step(
                            "Once user applies to it, new tab become available for the user.",
                            async c =>
                            {
                                c.Item.FeatureProposal.Apply();
                                c.Item.Feature.IsEnabled.ClaimTrue();
                            }
                        ),

                        new Step(
                            "Once user rejects it, new notification icon appeared in the settings.",
                            async c =>
                            {
                                c.Item.FeatureProposal.Reject();
                                c.Item.Feature.IsEnabled.ClaimFalse();

                                c.Item.Feature.IconVisible.ClaimTrue();
                            }
                        )
                    )
                )
            );

            var s = b.GetAllScenarios<Context>();

            var r = await s.ExecuteAll(() => new Context());

            r.WriteToOutput(Console.Write);
            r.FailIfOneOfScenariosFailed();
        }

        #endregion

        #region Step1

        [Test]
        public void TapOnNotification_ApplyToFeature()
        {
            var c = new Context();
            c.TapOnNotification();
            c.EnsureAppIsRunning();
            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }

        #endregion

        #region Step2

        [Test]
        public void TapOnNotification_AppIsClosed_LoggedIn_ApplyToFeature()
        {
            var c = new Context();
            c.PrepareLoggedInContext();

            c.EnsureAppIsNotRunning();
            c.TapOnNotification();
            c.EnsureAppIsRunning();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsRunning_LoggedIn_ApplyToFeature()
        {
            var c = new Context();
            c.PrepareLoggedInContext();

            c.EnsureAppIsRunning();

            c.TapOnNotification();
            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsClosed_NotLoggedIn_ApplyToFeature()
        {
            var c = new Context();
            c.PrepareNotLoggedInContext();

            c.EnsureAppIsNotRunning();
            c.TapOnNotification();
            c.EnsureAppIsRunning();

            c.EnsureNavigatedToLogin();
            c.PerformLogIn();
            c.EnsureUserIsLoggedIn();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsRunning_NotLoggedIn_ApplyToFeature()
        {
            var c = new Context();
            c.PrepareNotLoggedInContext();

            c.EnsureAppIsRunning();

            c.TapOnNotification();

            c.EnsureNavigatedToLogin();
            c.PerformLogIn();
            c.EnsureUserIsLoggedIn();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }

        #endregion

        #region Step3

        [Test]
        public void TapOnNotification_AppIsClosed_LoggedIn_RejectFeature()
        {
            var c = new Context();
            c.PrepareLoggedInContext();

            c.EnsureAppIsNotRunning();
            c.TapOnNotification();
            c.EnsureAppIsRunning();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Reject();
            c.Item.Feature.IsEnabled.ClaimFalse();

            c.Item.Feature.IconVisible.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsRunning_LoggedIn_RejectFeature()
        {
            var c = new Context();
            c.PrepareLoggedInContext();

            c.EnsureAppIsRunning();

            c.TapOnNotification();
            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Reject();
            c.Item.Feature.IsEnabled.ClaimFalse();

            c.Item.Feature.IconVisible.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsClosed_NotLoggedIn_RejectFeature()
        {
            var c = new Context();
            c.PrepareNotLoggedInContext();

            c.EnsureAppIsNotRunning();
            c.TapOnNotification();
            c.EnsureAppIsRunning();

            c.EnsureNavigatedToLogin();
            c.PerformLogIn();
            c.EnsureUserIsLoggedIn();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Reject();
            c.Item.Feature.IsEnabled.ClaimFalse();

            c.Item.Feature.IconVisible.ClaimTrue();
        }

        [Test]
        public void TapOnNotification_AppIsRunning_NotLoggedIn_RejectFeature()
        {
            var c = new Context();
            c.PrepareNotLoggedInContext();

            c.EnsureAppIsRunning();

            c.TapOnNotification();

            c.EnsureNavigatedToLogin();
            c.PerformLogIn();
            c.EnsureUserIsLoggedIn();

            c.EnsureNavigatedToFeatureProposal();
            c.Item.FeatureProposal.Reject();
            c.Item.Feature.IsEnabled.ClaimFalse();

            c.Item.Feature.IconVisible.ClaimTrue();
        }

        #endregion

        public class Step : IBehavior<Context>, INamedBehavior
        {
            private readonly string name;
            private readonly Func<Context, Task> action;

            public Step(string name, Func<Context, Task> action)
            {
                this.name = name;
                this.action = action;
            }

            public async Task Execute(Context context)
            {
                await action(context);
            }

            public string Name => name;
        }

        public class Context : Proxy<Application>
        {
            protected override Application Create()
            {
                return new Application();
            }


            public void TapOnNotification()
            {
            }

            public void EnsureAppIsRunning()
            {
            }

            public void EnsureNavigatedToFeatureProposal()
            {
            }

            public void EnsureUserIsLoggedIn()
            {
            }

            public void PrepareLoggedInContext()
            {
            }

            public void PrepareNotLoggedInContext()
            {
            }

            public void EnsureNavigatedToLogin()
            {
            }

            public void PerformLogIn()
            {
            }

            public void EnsureAppIsNotRunning()
            {
            }
        }

        public class Application
        {
            public Application()
            {
                FeatureProposal = new FeatureProposalPrompt(Feature);
            }

            public FeatureProposalPrompt FeatureProposal { get; }

            public NewFeature Feature { get; } = new NewFeature();

            public class FeatureProposalPrompt
            {
                private readonly NewFeature feature;

                public FeatureProposalPrompt(NewFeature feature)
                {
                    this.feature = feature;
                }

                public void Apply()
                {
                    feature.IsEnabled = true;
                    feature.IconVisible = false;
                }

                public void Reject()
                {
                    feature.IsEnabled = false;
                    feature.IconVisible = true;
                }
            }

            public class NewFeature
            {
                public bool IsEnabled { get; set; } = false;
                public bool IconVisible { get; set; } = false;
            }
        }
    }
}