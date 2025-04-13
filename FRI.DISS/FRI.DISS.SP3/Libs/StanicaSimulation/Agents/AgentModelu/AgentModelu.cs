using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu
{
	/*!
	 * Tiez by som ho považoval za agenta celej stanice
	 */
	//meta! id="1"
	public class AgentModelu : OSPABA.Agent
	{
        public Statistics StatisticsCustomerInStationTime { get; set; } = new();
        public Statistics StatisticsCustomerInQueueTime { get; set; } = new();


        private List<Customer> _customers = new List<Customer>();
        public List<Customer> Customers => _customers;
        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

		public AgentModelu(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

            // vycisti statistiky
            Customers.Clear();
            StatisticsCustomerInStationTime = new Statistics();
            StatisticsCustomerInQueueTime = new Statistics();
            Customer.IdCounter = 0;

            // odstartuj prichod zakaznikov
            var stanicaMesasge = ((MySimulation)MySim).CreateStanicaMessage();
            stanicaMesasge.Code = Mc.NoticeInicializuj;
            stanicaMesasge.Addressee = this;
            MyManager.Notice(stanicaMesasge);
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerModelu(SimId.ManagerModelu, MySim, this);
			AddOwnMessage(Mc.NoticePrichodZakaznika);
			AddOwnMessage(Mc.NoticeOdchodZakaznika);
		}
		//meta! tag="end"
	}

    public class Customer
    {
        public static int IdCounter = 0;
        public int Id { get; init; } = ++IdCounter;
        public double ArrivalTime { get; init; }
        public double? DepartureTime { get; set; } = null;


        public double? EnqueueTime { get; set; } = null;
        public double? ServiceTimeStart { get; set; } = null;
        public double? ServiceTimeEnd { get; set; } = null;
        
        public double QueueTime => ServiceTimeStart - EnqueueTime ?? 0;
        public double ServiceTime => ServiceTimeEnd - ServiceTimeStart ?? 0;
        public double TotalTime => DepartureTime - ArrivalTime ?? 0;

        public bool IsInService => ServiceTimeStart != null && ServiceTimeEnd == null;
        public bool IsServiced => ServiceTimeStart != null && ServiceTimeEnd != null;

        public CustomerState State { get; set; } = CustomerState.Entered;

        public override string ToString()
        {
            return $"#{Id} arrived at {ArrivalTime}";
        }
    }

    public enum CustomerState
    {
        Entered,
        InQueue,
        InService,
        Serviced,
        Left
    }
}