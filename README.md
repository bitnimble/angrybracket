# angrybracket
A set of helper classes and extension methods for .NET

# Overview of classes

Note that all of the classes (or at least the extension method classes -- datastructures may be moved into another namespace in future) are actually in the root namespace, so you don't have to go digging around for the proper namespace to include. Just include `using AngryBracket;` and you'll get all of the helper extension methods available instantly. 

### Controls

|     Class     | Description |
|     -----     | ----------- |
| GhostTextbox  | Like a normal TextBox control, but supports "ghost" placeholder text through the GhostText property. |
| ThemedListView| A ListView, but with the native Windows explorer theming instead of the god-awful XP style block highlighting. Gives you the nice hover animations and effects (screenshots below). |

![](https://my.mixtape.moe/iwkkfa.gif) ![](https://my.mixtape.moe/oefowu.gif)

### Datastructures

|     Class     | Description |
|     -----     | ----------- |
| CircularBuffer| Implementation of a [circular buffer](https://en.wikipedia.org/wiki/Circular_buffer). Like a stack, but you can just keep pushing. Once you push more items than its capacity, it'll start overwriting from the beginning. Note: can't pop yet, since I only really use it for AveragingBuffer (see below). |
| AveragingBuffer| Push stuff into it, and it'll keep a running average. Great for uses such as FPS timers (just push in your frametimes, and divide 1000 by the CurrentAverage). Note that it has a "spin up time" when pushing the first `size` values, where the average will slowly increase to the true average. Decreasing the size of the buffer will reduce this time, but also make the average more sensitive to changes. |
| PartialList | Provides a "view" into a List, when you need to pass in part of a List into a method, but don't want to copy the values into another List. |

### Helpers

|     Class     | Description |
|     -----     | ----------- |
| ArrayHelper | Extension methods for Arrays. |
| CollectionsHelper | Extension methods for some ICollection classes (Stack, Queue, Dictionary, etc). |
| DrawingHelper | Extension methods for System.Drawing, and datastructures such as Point, Rectangle, Size, etc. |
| FileHelper | Extension methods for file and directory related classes. |
| IEnumerableHelper | Extension methods for IEnumerables. |
| IOHelper | Extension methods for Streams and Readers/Writers. |
| JsonHelper | A tiny helper method for parsing and serialising Json, wrapping the System.Runtime.Serialization standard library. No clue about the performance. |
| MathHelper | Provides extension methods for number types (int, long, float, etc). |
| NativeMethods | A collection of native win32 methods used by some of the classes in AngryBracket. You probably won't use these. |
| NaturalSortComparer | An IComparer that sorts things alphabetically in the common sense way. See [here](https://en.wikipedia.org/wiki/Natural_sort_order) for info. |
| ProcessHelper | Extension and static methods for System.Diagnostics and other Process related methods. |
| RandomUtil | A static, thread-safe version of Random so you don't have to init one yourself. Also includes a cryptographically secure random generator version. |
| ReflectionHelper | Extension methods for System.Reflection. |
| StringHelper | Extension methods for the String class, including date parsing and faster ToString ("ToStringQuick") functions. |
| ThreadHelper | Helper methods relating to System.Threading. Currently only includes an IEnumerable extension method to multithread workloads easily. |
| WebHelper | Static helper methods for System.Net and System.Web. Currently only includes a Post and Get method. | 


More documentation coming soon (and better extension method names...)
