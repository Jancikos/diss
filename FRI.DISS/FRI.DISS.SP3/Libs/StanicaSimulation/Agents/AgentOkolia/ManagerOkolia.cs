using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;


namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia
{
	//meta! id="3"
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


		//meta! sender="SchedulerPrichodZakaznika", id="34", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
                case Mc.NoticePrichodZakaznika:
                    ProcessNoticePrichodZakaznika(message);
                    break;
			}
		}

        private void ProcessNoticePrichodZakaznika(MessageForm message)
        {
            if (message.Code != Mc.NoticePrichodZakaznika)
            {
                throw new InvalidOperationException("Invalid message code");
            }

            // notifikuj parku o prichode zakaznika
            message.Addressee = MyAgent.Parent;
            Notice(message.CreateCopy());

            // naplanuj prichod dalsieho zakaznika
            message.Addressee = MyAgent.FindAssistant(SimId.SchedulerPrichodZakaznika);
            StartContinualAssistant(message.CreateCopy());
        }

        //meta! sender="AgentModelu", id="59", type="Notice"
        public void ProcessNoticeInicializuj(MessageForm message)
		{
            // spusti SchedulerPrichodZakaznika pre generovanie prichodov zakaznikov
            message.Addressee = MyAgent.FindAssistant(SimId.SchedulerPrichodZakaznika);
            StartContinualAssistant(message.CreateCopy());
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

			case Mc.NoticeInicializuj:
				ProcessNoticeInicializuj(message);
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