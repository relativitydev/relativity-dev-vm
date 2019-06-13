using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Helpers
{
	public class YmlFileHelper : IYmlFileHelper
	{
		public void UpdateElasticSearchYml()
		{
			//string pathToYmlFile = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml";
			string pathToYmlFile = @"C:\Users\aaron.gilbert\Desktop\elasticsearch2.yml";
			if (!File.Exists(pathToYmlFile))
			{
				throw new Exception("File does not exist");
			}

			string stream = File.ReadAllText(pathToYmlFile);

			StringReader input = new StringReader(pathToYmlFile);

			IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(new NullNamingConvention()).Build();

			ElasticSearchYml order = deserializer.Deserialize<ElasticSearchYml>(input);

			Console.WriteLine("Yaml Converted");
		}
	}

	public class ElasticSearchYml
	{
		[YamlMember(Alias = "cluster.name", ApplyNamingConventions = false)]
		public string clusterName { get; set; }

		[YamlMember(Alias = "node.name", ApplyNamingConventions = false)]
		public string nodeName { get; set; }

		[YamlMember(Alias = "node.master", ApplyNamingConventions = false)]
		public bool nodeMaster { get; set; }

		[YamlMember(Alias = "node.data", ApplyNamingConventions = false)]
		public bool nodeData { get; set; }

		[YamlMember(Alias = "discovery.zen.minimum_master_nodes", ApplyNamingConventions = false)]
		public int discoveryZenMinimumMasterNodes { get; set; }

		[YamlMember(Alias = "discovery.zen.ping.multicast.enabled", ApplyNamingConventions = false)]
		public bool discoveryZenPingMulticastEnabled { get; set; }

		[YamlMember(Alias = "discovery.zen.ping.unicast.hosts", ApplyNamingConventions = false)]
		public string discoveryZenPingUnicastHosts { get; set; }

		[YamlMember(Alias = "marvel.agent.enabled", ApplyNamingConventions = false)]
		public bool marvelAgentEnabled { get; set; }

		[YamlMember(Alias = "action.destructive_requires_name", ApplyNamingConventions = false)]
		public bool actionDestructiveRequiresName { get; set; }

		[YamlMember(Alias = "action.auto_create_index", ApplyNamingConventions = false)]
		public string actionAutoCreateIndex { get; set; }

		[YamlMember(Alias = "format", ApplyNamingConventions = false)]
		public string format { get; set; }

		[YamlMember(Alias = "transport.tcp.compress", ApplyNamingConventions = false)]
		public bool transportTcpCompress { get; set; }

		[YamlMember(Alias = "http.max_content_length", ApplyNamingConventions = false)]
		public string httpMaxContentLength { get; set; }

		[YamlMember(Alias = "http.cors.enabled", ApplyNamingConventions = false)]
		public bool httpCorsEnabled { get; set; }

		[YamlMember(Alias = "gateway.expected_master_nodes", ApplyNamingConventions = false)]
		public int gatewayExpectedMasterNodes { get; set; }

		[YamlMember(Alias = "gateway.expected_data_nodes", ApplyNamingConventions = false)]
		public int gatewayExpectedDataNodes { get; set; }

		[YamlMember(Alias = "gateway.recover_after_time", ApplyNamingConventions = false)]
		public string gatewayRecoverAfterTime { get; set; }

		[YamlMember(Alias = "script.default_lang", ApplyNamingConventions = false)]
		public string scriptDefaultLang { get; set; }

		[YamlMember(Alias = "script.groovy.sandbox.enabled", ApplyNamingConventions = false)]
		public bool scriptGroovySandboxEnabled { get; set; }

		[YamlMember(Alias = "path.data", ApplyNamingConventions = false)]
		public string pathData { get; set; }

		[YamlMember(Alias = "path.repo", ApplyNamingConventions = false)]
		public string pathRepo { get; set; }

		[YamlMember(Alias = "index.unassigned.node_left.delayed_timeout", ApplyNamingConventions = false)]
		public string indexUnassignedNodeLeftDelayedTimeout { get; set; }

		[YamlMember(Alias = "script.inline", ApplyNamingConventions = false)]
		public string scriptInline { get; set; }

		[YamlMember(Alias = "script.indexed", ApplyNamingConventions = false)]
		public string scriptIndexed { get; set; }

		[YamlMember(Alias = "security.manager.enabled", ApplyNamingConventions = false)]
		public bool securityManagerEnabled { get; set; }

		[YamlMember(Alias = "network.host", ApplyNamingConventions = false)]
		public string networkHost { get; set; }

		[YamlMember(Alias = "index.max_result_window", ApplyNamingConventions = false)]
		public long indexMaxResultWindow { get; set; }

		[YamlMember(Alias = "shield.enabled", ApplyNamingConventions = false)]
		public bool shieldEnabled { get; set; }

		[YamlMember(Alias = "shield.authc.realms", ApplyNamingConventions = false)]
		public ShieldAuthcRealms shieldAuthcRealms { get; set; }
	}

	public class ShieldAuthcRealms
	{
		public custom custom { get; set; }
		public esusers1 esusers1 { get; set; }
	}

	public class custom
	{
		public string type { get; set; }
		public int order { get; set; }
		[YamlMember(Alias = "publicJWKsUrl", ApplyNamingConventions = false)]
		public string publicJWKsUrl { get; set; }
	}

	public class esusers1
	{
		public string type { get; set; }
		public int order { get; set; }
	}
}
