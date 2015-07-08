# AsyncPoco (Bamboo edition)

## What's AsyncPoco

AsyncPoco is a fork of the popular [PetaPoco](http://www.toptensoftware.com/petapoco) micro-ORM for .NET, with a fully asynchronous API and support for the async/await keywords in C# 5.0 and VB 11. It does not supercede PetaPoco; the two can peacefully co-exist in the same project. When making the decision to go asynchronous, it's generally best to go "all in", but keeping both around can be helpful while making a gradual transition.

## Credit where credit is due
This project is forked from https://github.com/tmenier/AsyncPoco in 2016

## Tested .Net version
.NET Framework 4.8

## Is it faster?
Yes if you use multiple threads for tasks.

## Fork Improvements
Instead of using "DBContext.query," it now allows the alternative "entity.Save(DB context)" syntax for snap-in transaction. DB contexts are created and shared across threads via ".GetInstance()". If a transaction is needed, you should create a non-shared context using "new DBContext()" and manage its lifespan.

## Not Implemented
- /AsyncPocoCore is at work

## Recommend usage and precaution.
1. Don't use combo primary-keys. (support is complex and weak)
2. Read context is free to be shared. Each thread manage its own shared context. Keep writing context to yourself and don't share it through async Task preferably with mini-"using" here and there. If you need transaction/write across Tasks, create a dedicated new context for it.
