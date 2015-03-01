<img src="https://cdn.rawgit.com/zeromq/netmq/master/img/NetMQLogo.svg" width="350" />

[![NetMQ Team City Build](https://img.shields.io/teamcity/codebetter/bt1046.svg)](http://teamcity.codebetter.com/project.html?projectId=NetMQ) [![NetMQ NuGet version](https://img.shields.io/nuget/v/NetMQ.svg)](https://www.nuget.org/packages/NetMQ/) [![NetMQ download stats](https://img.shields.io/nuget/dt/NetMQ.svg)](https://www.nuget.org/packages/NetMQ/)

NetMQ is a 100% native C# port of the lightweight messaging library ZeroMQ.

NetMQ extends the
standard socket interfaces with features traditionally provided by
specialised messaging middleware products. NetMQ sockets provide an
abstraction of asynchronous message queues, multiple messaging patterns,
message filtering (subscriptions), seamless access to multiple transport
protocols, and more.

## Installation

You can download NetMQ via [NuGet](https://nuget.org/packages/NetMQ/).

## Using / Documentation

Before using NetMQ, make sure to read the [ZeroMQ Guide](http://zguide.zeromq.org/page:all).

The NetMQ documentation can be found at [netmq.readthedocs.org](http://netmq.readthedocs.org/en/latest/). Thanks to [Sacha Barber](http://www.codeproject.com/Members/Sacha-Barber) who agreed to do the documentation.


There are also a few blog posts available, which you can read about here:

+ [Somdorons blog](http://somdoron.com/category/netmq/)
+ [Hello World](http://sachabarbs.wordpress.com/2014/08/19/zeromq-1-introduction/)
+ [The Socket Types](http://sachabarbs.wordpress.com/2014/08/21/zeromq-2-the-socket-types-2/)
+ [Socket Options/Identity and SendMore](http://sachabarbs.wordpress.com/2014/08/26/zeromq-3-socket-optionsidentity-and-sendmore/)
+ [Multiple Socket Polling](http://sachabarbs.wordpress.com/2014/08/27/zeromq-4-multiple-sockets-polling/)
+ [Sending From Multiple Sockets](https://sachabarbs.wordpress.com/2014/08/30/zeromq-sending-from-multiple-sockets/)
+ [Divide And Conquer](http://sachabarbs.wordpress.com/2014/09/01/zeromq-6-divide-and-conquer/)


Here is a simple example:

```csharp
using (var context = NetMQContext.Create())
using (var server = context.CreateResponseSocket())
using (var client = context.CreateRequestSocket())
{
    // Bind the server to a local TCP address
    server.Bind("tcp://localhost:5556");

    // Connect the client to the server
    client.Connect("tcp://localhost:5556");

    // Send a message from the client socket
    client.Send("Hello");

    // Receive the message from the server socket
    string m1 = server.ReceiveString();
    Console.WriteLine("From Client: {0}", m1);

    // Send a response back from the server
    server.Send("Hi Back");

    // Receive the response from the client socket
    string m2 = client.ReceiveString();
    Console.WriteLine("From Server: {0}", m2);
}
```

## Contributing

We need help, so if you have good knowledge of C# and ZeroMQ just grab one of the issues and add a pull request.
We are using [C4.1 process](http://rfc.zeromq.org/spec:22), so make sure you read this before.

Regarding coding standards, we are using C# coding styles, to be a little more specific: we are using `camelCase` for variables and fields (with `m_` prefix for instance members and `s_` for static fields) and `PascalCase` for methods, classes and constants. Make sure you are using 'Insert Spaces' and 4 for tab and indent size.

You can also help us by:

* Joining our [mailing list](https://groups.google.com/d/forum/netmq-dev?hl=en) and be an active member
* Writing tutorials in the github wiki
* Writing about the project in your blog (and add a pull request with a link to your blog at the bottom of this page)

## Important note on backward compatibility 

Since version 3.3.07 NetMQ changed the number serialization from Little Endian to Big Endian to be compatible with ZeroMQ.
Any NetMQ version prior to 3.3.0.7 is not compatible with the new version. To support older versions you can set Endian option on a NetMQ socket to Little Endian,
however doing so will make it incompatible with ZeroMQ.

We recommend to update to the latest version and use Big Endian which is now the default behavior.

## Mailing list

You can join our mailing list [here](https://groups.google.com/d/forum/netmq-dev?hl=en). 

## Who owns NetMQ?

NetMQ is owned by all its authors and contributors. 
This is an open source project licensed under the LGPLv3. 
To contribute to NetMQ please read the [C4.1 process](http://rfc.zeromq.org/spec:22), it's what we use.
There are open issues in the issues tab that still need to be taken care of, feel free to pick one up and submit a patch to the project.

## Build Server

[TeamCity at CodeBetter](http://teamcity.codebetter.com/project.html?projectId=project372&tab=projectOverview)

![Code Better](http://www.jetbrains.com/img/banners/Codebetter300x250.png)

[YouTrack by JetBrains - keyboard-centric bug tracker](http://www.jetbrains.com/youtrack)

[TeamCity by JetBrains - continuous integration server](http://www.jetbrains.com/teamcity)
