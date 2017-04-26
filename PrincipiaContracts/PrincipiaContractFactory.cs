using ContractConfigurator;
using Contracts;
using UnityEngine;

namespace PrincipiaContracts
{
	public class PrincipiaContractFactory : ParameterFactory
	{

		protected string locationName;
		protected double maxDistance;
		protected double minDistance;

		public override bool Load(ConfigNode configNode)
		{
			bool valid = base.Load(configNode);


			valid &= ConfigNodeUtil.ParseValue<string>(configNode, "locationName", x => locationName = x, this);
			valid &= ConfigNodeUtil.ParseValue(configNode, "maxDistance", x => maxDistance = x, this, double.MaxValue, x => Validation.GE(x, 0.0));
			valid &= ConfigNodeUtil.ParseValue(configNode, "minDistance", x => minDistance = x, this, 0.0, x => Validation.GE(x, 0.0));

			return valid;
		}
		public override ContractParameter Generate(Contract contract)
		{
			return new PrincipiaContractParameter(locationName, minDistance, maxDistance);
		}
	}
}
