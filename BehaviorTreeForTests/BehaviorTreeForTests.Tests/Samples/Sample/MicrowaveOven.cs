using System.Reactive.Subjects;
using Medbullets.CrossCutting.Extensions;

namespace BehaviorTreeForTests.Tests.Samples.Sample
{
    public class MicrowaveOven
    {
        public PowerTube PowerTube { get; } = new PowerTube();

        public TimeSpan Remains { get; private set; } = 0.Milliseconds();

        public Light Light { get; } = new Light();

        public Beeper Beeper { get; } = new Beeper();

        public IObservable<bool> IsWorking { get; } = new BehaviorSubject<bool>(false);

        public void PushTheButton()
        {
            Remains = 1.Minutes();
            PowerTube.IsEnergised = true;
        }

        public void OpenTheDoor()
        {
        }

        public void CloseTheDoor()
        {
        }
    }
}