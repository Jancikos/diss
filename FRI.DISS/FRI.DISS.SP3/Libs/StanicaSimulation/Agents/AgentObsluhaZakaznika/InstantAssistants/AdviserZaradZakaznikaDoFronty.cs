using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.InstantAssistants
{
	//meta! id="40"
	public class AdviserZaradZakaznikaDoFronty : OSPABA.Adviser
	{
		public AdviserZaradZakaznikaDoFronty(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void Execute(MessageForm message)
		{
		}
		public new AgentObsluhaZakaznika MyAgent
		{
			get
			{
				return (AgentObsluhaZakaznika)base.MyAgent;
			}
		}
	}
}