using System.Runtime.CompilerServices;
using Unity.Properties;

[assembly: InternalsVisibleTo("Noneb.AI.PlayModeTests")]
[assembly: InternalsVisibleTo("Noneb.AI.Editor")]
// This assembly is the default dynamic assembly generated Castle DynamicProxy, 
// used by Moq. Paste in a single line.   
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: GeneratePropertyBagsForAssembly]

//todo: if we can work out how switch works, we can probably have switch like structure spit out action name and do name matching to pick the action.

/*
 * action node to set blackboard, use children to find value.
 * condition branching to match it?#
 * our custom interface can give it name?
 * SwitchNodeModel - we can technically hijack it?
 *
 * sol -> extend composite!
 * how do we pass in context object?
 *
 * inspect -> set blackboard list of kv pair for score lookup.
 */