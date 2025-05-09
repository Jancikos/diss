using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.ContinualAssistants;
using FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu;
using FRI.DISS.Libs.Generators;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika
{
	//meta! id="19"
	public class ManagerObsluhaZakaznika : OSPABA.Manager
	{
		public ManagerObsluhaZakaznika(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentStanica", id="28", type="Request"
		public void ProcessRequestResponseObluzZakaznika(MessageForm message)
		{
            var myMsg = (MyStanicaMesasge)message.CreateCopy();

            // ak je stanica obsadena, tak zakaznika zarad do fronty
            if (!MyAgent.IsServiceFree)
            {
                myMsg.Customer!.State = CustomerState.InQueue;
                myMsg.Customer!.EnqueueTime = MySim.CurrentTime;
                MyAgent.CustomersQueue.Enqueue(myMsg.Customer!);
                return;
            }

            myMsg.Customer!.State = CustomerState.InService;
            myMsg.Customer!.ServiceTimeStart = MySim.CurrentTime;

            MyAgent.IsServiceFree = false;

            message.Addressee = MyAgent.FindAssistant(SimId.ProcessObsluzZakaznika);
            StartContinualAssistant(message);
		}

		//meta! sender="ProcessObsluzZakaznika", id="39", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
            // ukonci obsliuhu zakaznika
            var myMsg = (MyStanicaMesasge)message.CreateCopy();
            var customer = myMsg.Customer!;

            customer.ServiceTimeEnd = MySim.CurrentTime;
            customer.State = CustomerState.Serviced;
            MyAgent.IsServiceFree = true;

            // posli ho na presun von zo stanice
            myMsg.Code = Mc.RequestResponsePresunZakaznika;
            Response(myMsg);

            // skus naplanovat obsluhu dalsieho zakaznika
            if (MyAgent.CustomersQueue.Count > 0)
            {
                // spusti obsluhu dalsieho zakaznika
                var nextCustomer = MyAgent.CustomersQueue.Dequeue();
				//var nextMsg = new MyStanicaMesasge(MySim);
				var nextMsg =  (MyStanicaMesasge)message.CreateCopy();
                nextMsg.Customer = nextCustomer;
                nextMsg.Code = Mc.RequestResponseObluzZakaznika;
                nextMsg.Addressee = this;
                ProcessRequestResponseObluzZakaznika(nextMsg);
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
			case Mc.Finish:
				ProcessFinish(message);
			break;

			case Mc.RequestResponseObluzZakaznika:
				ProcessRequestResponseObluzZakaznika(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentObsluhaZakaznika MyAgent
		{
			get
			{
				return (AgentObsluhaZakaznika)base.MyAgent;
			}
		}
	}
}