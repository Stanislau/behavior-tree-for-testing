# Behavior Tree for Testing

[![PDD status](https://www.0pdd.com/svg?name=Stanislau/behavior-tree-for-testing)](https://www.0pdd.com/p?name=Stanislau/behavior-tree-for-testing)

# Problem

Integration testing is a mechanism to ensure that functionality works as expected against the requirements.
Usually we use BDD approach with given-when-then syntax to describe desired behavior and trying to automate each case separately.
It looks like:

N1
Given microwave is closed and idle
When user taps the button
Then microwave should start

N2
Given microwave is closed and idle
When user taps the button
When user taps the button again
Then microwave displays 2 minutes

As it is definitely seen we have at least 50% flow duplication here.

As the result when we have tons of such specs, we have tons of duplication, loses visibility and maintainability of tests.
They harder to maintain -> we deside to write them less -> quality suffers.

# Solution

As developers, we do not describe algorithm by all step by step variations of execution.

var a = 5;
if (b == 3)
{
	a = 3;
}

It means that we do not describe it like:
N1:
a = 5;
b = 4;
N2:
a = 5;
b = 3;
a = 3;

We tend to see the whole picture, the entire algorithm.

For this there are https://en.wikipedia.org/wiki/Behavior_tree approach.
This approach is a way to maintain requirements inside executable tree, describing the entire flow.
Even more about this is here https://staff.itee.uq.edu.au/kirsten/publications/BTMergeSemantics_withCopyright.pdf.

# Implementation

A library to define a tree with requirements:

```cs
await new BehaviorTree(
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
.Dump()
.ExecuteAll(() => new MicrowaveOvenContext());
```

## Sequence

Step by step execution. One sequence generates only one flow.

## Branching

Every child in a branching element creates separate flow, only one branching element can be presented in a flow.

## Leaf

Every leaf element is either action or requirement check.

# Possible extends

Visual tool to build the tree in a user friendly way so customer or BA can use it as a tool and then, similar to BDD approach, developer can automate it in form of tests.

Behavior tree can be used by developers to extract modules from requirements, applying real Behavior Driven Development into a project.

/*
* @todo #:20m/Arch Create a document describing presentation plan and decompose it into steps.
 * PresentationPlan.md with presentation steps.
*/

/*
* @todo #:30m/Dev Setup solution with core library, tests and example project.
*/

/*
* @todo #:30m/Dev test todo to check how PDD works.
*/
