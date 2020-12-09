using Shadowsocks.Interop.V2Ray.Routing;
using System.Collections.Generic;

namespace Shadowsocks.Interop.V2Ray
{
    public class RoutingObject
    {
        /// <summary>
        /// Gets or sets the domain strategy used for routing.
        /// Default value: AsIs.
        /// Available values: "AsIs" | "IPIfNonMatch" | "IPOnDemand"
        /// </summary>
        public string DomainStrategy { get; set; }

        /// <summary>
        /// Gets or sets the list of routing rules.
        /// </summary>
        public List<RuleObject> Rules { get; set; }

        /// <summary>
        /// Gets or sets the list of load balancers.
        /// </summary>
        public List<BalancerObject>? Balancers { get; set; }

        public RoutingObject()
        {
            DomainStrategy = "AsIs";
            Rules = new();
        }

        public static RoutingObject DefaultBalancers => new()
        {
            Balancers = new(),
        };
    }
}
