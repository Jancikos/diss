using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika;
using FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentStanica
{
	//meta! id="5"
	public class ManagerStanica : OSPABA.Manager
	{
		public ManagerStanica(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentModelu", id="23", type="Notice"
		public void ProcessNoticePrichodZakaznika(MessageForm message)
		{
            MyAgent.CustomersInStation++;

            // presun zakaznika ku obsluhe
            _presunZakaznika((MyStanicaMesasge)message);
		}

		//meta! sender="AgentObsluhaZakaznika", id="28", type="Response"
		public void ProcessRequestResponseObluzZakaznika(MessageForm message)
		{
            // presun zakaznika von zo stanice
            _presunZakaznika((MyStanicaMesasge)message);
		}

        protected void _presunZakaznika(MyStanicaMesasge message)
        {
            // presun zakaznika von zo stanice
            var mesPresunu = message.CreateCopy();
            mesPresunu.Code = Mc.RequestResponsePresunZakaznika;
            mesPresunu.Addressee = MySim.FindAgent(SimId.AgentPresunZakaznika);
            Request(mesPresunu);
        }
        

		/*!
		 * Z dveri ku podladni a naopak
		 */
		//meta! sender="AgentPresunZakaznika", id="27", type="Response"
		public void ProcessRequestResponsePresunZakaznika(MessageForm message)
		{
            var myMsg = (MyStanicaMesasge)message;

            switch (myMsg.Customer!.State)
            {
                case CustomerState.Entered:
                    // naplnuj obsluhu
                    var mesObsluhy = myMsg.CreateCopy();
                    mesObsluhy.Code = Mc.RequestResponseObluzZakaznika;
                    mesObsluhy.Addressee = MySim.FindAgent(SimId.AgentObsluhaZakaznika);
                    Request(mesObsluhy);
                    break;
                case CustomerState.Serviced:
                    MyAgent.CustomersInStation--;

                    // zakaznik je von zo stanice
                    var mesOdchodu = myMsg.CreateCopy();
                    mesOdchodu.Code = Mc.NoticeOdchodZakaznika;
                    mesOdchodu.Addressee = MySim.FindAgent(SimId.AgentModelu);
                    Notice(mesOdchodu);
                    break;
            }
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
			case Mc.RequestResponseObluzZakaznika:
				ProcessRequestResponseObluzZakaznika(message);
			break;

			case Mc.NoticePrichodZakaznika:
				ProcessNoticePrichodZakaznika(message);
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
		public new AgentStanica MyAgent
		{
			get
			{
				return (AgentStanica)base.MyAgent;
			}
		}
	}
}