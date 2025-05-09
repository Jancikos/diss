using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia;
using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia.InstantAssistants
{
	//meta! id="35"
	public class AdviserOdchodZakaznika : OSPABA.Adviser
	{
		public AdviserOdchodZakaznika(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void Execute(MessageForm message)
		{
		}
		public new AgentOkolia MyAgent
		{
			get
			{
				return (AgentOkolia)base.MyAgent;
			}
		}
	}
}
