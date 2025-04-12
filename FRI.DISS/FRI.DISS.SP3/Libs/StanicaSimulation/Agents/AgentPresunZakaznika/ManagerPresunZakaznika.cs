using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika
{
	//meta! id="18"
	public class ManagerPresunZakaznika : OSPABA.Manager
	{
		public ManagerPresunZakaznika(int id, OSPABA.Simulation mySim, Agent myAgent) :
			base(id, mySim, myAgent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

			if (PetriNet != null)
			{
				PetriNet.Clear();
			}
		}

		/*!
		 * Z dveri ku podladni a naopak
		 */
		//meta! sender="AgentStanica", id="27", type="Request"
		public void ProcessRequestResponsePresunZakaznika(MessageForm message)
		{
		}

		//meta! sender="ProcessPresunZakaznika", id="43", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Finish:
				ProcessFinish(message);
			break;

			case Mc.RequestResponsePresunZakaznika:
				ProcessRequestResponsePresunZakaznika(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentPresunZakaznika MyAgent
		{
			get
			{
				return (AgentPresunZakaznika)base.MyAgent;
			}
		}
	}
}
