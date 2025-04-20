using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia
{
	//meta! id="2"
	public class ManagerOkolia : OSPABA.Manager
	{
		public ManagerOkolia(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentModelu", id="9", type="Notice"
		public void ProcessNoticeInicialuzuj(MessageForm message)
		{
            message.Addressee = MyAgent.FindAssistant(SimId.SchedulerPrichody);
            StartContinualAssistant(message.CreateCopy());
		}

		//meta! sender="SchedulerPrichody", id="69", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
            // notifikuj AgentaModelu o prichode objednavky
            message.Code = Mc.NoticePrichodObjednavka;
            message.Addressee = MyAgent.Parent;
            Notice(message.CreateCopy());

            // naplanuj prichod dalsieho zakaznika
            message.Addressee = MyAgent.FindAssistant(SimId.SchedulerPrichody);
            StartContinualAssistant(message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.NoticeInicialuzuj:
				ProcessNoticeInicialuzuj(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentOkolia MyAgent
		{
			get
			{
				return (AgentOkolia)base.MyAgent;
			}
		}
	}
}
