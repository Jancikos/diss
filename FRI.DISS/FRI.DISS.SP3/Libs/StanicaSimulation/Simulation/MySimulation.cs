using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia;
using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentStanica;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Simulation
{
	public class MySimulation : OSPABA.Simulation
	{   
        public Statistics? CustomersCount;
        public Statistics? CustomersInStationLeftCount;


		public MySimulation()
		{
			Init();
		}

		override public void PrepareSimulation()
		{
			base.PrepareSimulation();

			// Create global statistics
			CustomersCount = new Statistics();
            CustomersInStationLeftCount = new Statistics();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Reset entities, queues, local statistics, etc...

            var agentModelu = (AgentModelu)FindAgent(SimId.AgentModelu);

            agentModelu.Customers.Clear();
            Customer.IdCounter = 0;

            var agentStanice = (AgentStanica)FindAgent(SimId.AgentStanica);
            agentStanice.CustomersInStation = 0;
		}

		override public void ReplicationFinished()
		{
			// Collect local statistics into global, update UI, etc...
			base.ReplicationFinished();

            var agentModelu = (AgentModelu)FindAgent(SimId.AgentModelu);
            CustomersCount!.AddSample(agentModelu.Customers.Count);

            var agentStanice = (AgentStanica)FindAgent(SimId.AgentStanica);
            CustomersInStationLeftCount!.AddSample(agentStanice.CustomersInStation);
		}

		override public void SimulationFinished()
		{
			// Display simulation results
			base.SimulationFinished();
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			AgentModelu = new AgentModelu(SimId.AgentModelu, this, null);
			AgentOkolia = new AgentOkolia(SimId.AgentOkolia, this, AgentModelu);
			AgentStanica = new AgentStanica(SimId.AgentStanica, this, AgentModelu);
			AgentPresunZakaznika = new AgentPresunZakaznika(SimId.AgentPresunZakaznika, this, AgentStanica);
			AgentObsluhaZakaznika = new AgentObsluhaZakaznika(SimId.AgentObsluhaZakaznika, this, AgentStanica);
		}
		public AgentModelu AgentModelu
		{ get; set; }
		public AgentOkolia AgentOkolia
		{ get; set; }
		public AgentStanica AgentStanica
		{ get; set; }
		public AgentPresunZakaznika AgentPresunZakaznika
		{ get; set; }
		public AgentObsluhaZakaznika AgentObsluhaZakaznika
		{ get; set; }
		//meta! tag="end"

        public MyStanicaMesasge CreateStanicaMessage()
        {
            return new MyStanicaMesasge(this);
        }
	}
    
    public class MyStanicaMesasge : MessageForm
    {
        public Customer? Customer { get; set; } = null;

        public MyStanicaMesasge(OSPABA.Simulation mySim) : base(mySim)
        {
        }

        public override MessageForm CreateCopy()
        {
            var copy = new MyStanicaMesasge(MySim);
            
            copy.Copy(this);

            copy.Customer = Customer;

            return copy;
        }
    }
}