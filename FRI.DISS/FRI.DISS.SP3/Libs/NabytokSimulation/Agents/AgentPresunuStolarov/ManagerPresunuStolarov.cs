using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov
{
	/*!
	 * bude primat 2 ReqRRes
	 * 
	 * presun medzi skldom a wp
	 * presun mediz wp a wp 
	 */
	//meta! id="54"
	public class ManagerPresunuStolarov : OSPABA.Manager
	{
		public ManagerPresunuStolarov(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentStolarov", id="61", type="Request"
		public void ProcessRequestResponsePresunSklad(MessageForm message)
		{
		}

		//meta! sender="AgentStolarov", id="60", type="Request"
		public void ProcessRequestResponsePresunPracoviska(MessageForm message)
		{
		}

		//meta! sender="ProcessPresunPracoviska", id="65", type="Finish"
		public void ProcessFinishProcessPresunPracoviska(MessageForm message)
		{
		}

		//meta! sender="ProcessPresunSklad", id="63", type="Finish"
		public void ProcessFinishProcessPresunSklad(MessageForm message)
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
			case Mc.RequestResponsePresunPracoviska:
				ProcessRequestResponsePresunPracoviska(message);
			break;

			case Mc.Finish:
				switch (message.Sender.Id)
				{
				case SimId.ProcessPresunPracoviska:
					ProcessFinishProcessPresunPracoviska(message);
				break;

				case SimId.ProcessPresunSklad:
					ProcessFinishProcessPresunSklad(message);
				break;
				}
			break;

			case Mc.RequestResponsePresunSklad:
				ProcessRequestResponsePresunSklad(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentPresunuStolarov MyAgent
		{
			get
			{
				return (AgentPresunuStolarov)base.MyAgent;
			}
		}
	}
}
