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
        
		protected void _startPresun(MessageForm message, int assistantId)
		{
            var myMsg = (MyMessage)message;

            var stolar = myMsg.Stolar!;
            var oldPracovisko = stolar.CurrentPracovisko!;

            // update position of the stolar
            stolar.CurrentPracovisko = null; // presun
            oldPracovisko.Stolari.Remove(stolar.Id); // remove from old

            // vykonaj presun
            myMsg.Addressee = MyAgent.FindAssistant(assistantId);
            StartContinualAssistant(myMsg);
		}

        protected void _endPresun(MessageForm message, int responseMsgCode)
        {
            var myMsg = (MyMessage)message;
            var stolar = myMsg.Stolar!;
            var newPracovisko = myMsg.Pracovisko!;

            // update position of the stolar
            stolar.CurrentPracovisko = newPracovisko;
            newPracovisko.Stolari.Add(stolar.Id, stolar);

            // notify about the end of the operation
            myMsg.Code = responseMsgCode;
            Response(myMsg);
        }

		//meta! sender="AgentStolarov", id="61", type="Request"
		public void ProcessRequestResponsePresunSklad(MessageForm message)
		{
            _startPresun(message, SimId.ProcessPresunSklad);
		}

		//meta! sender="AgentStolarov", id="60", type="Request"
		public void ProcessRequestResponsePresunPracoviska(MessageForm message)
		{
            _startPresun(message, SimId.ProcessPresunPracoviska);
		}

		//meta! sender="ProcessPresunPracoviska", id="65", type="Finish"
		public void ProcessFinishProcessPresunPracoviska(MessageForm message)
		{
            _endPresun(message, Mc.RequestResponsePresunPracoviska);
		}

		//meta! sender="ProcessPresunSklad", id="63", type="Finish"
		public void ProcessFinishProcessPresunSklad(MessageForm message)
		{
            _endPresun(message, Mc.RequestResponsePresunSklad);
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
