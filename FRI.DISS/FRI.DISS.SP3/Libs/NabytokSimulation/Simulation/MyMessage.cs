using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using OSPABA;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
	public class MyMessage : OSPABA.MessageForm
	{
        public Objednavka? Objednavka { get; set; } = null;
        public Nabytok? Nabytok { get; set; } = null;
        public Pracovisko? Pracovisko { get; set; } = null;
        public Stolar? Stolar { get; set; } = null;
        public Queue<StolarType> StolarTypes { get; set; } = new();

		public MyMessage(OSPABA.Simulation mySim) :
			base(mySim)
		{
		}

		public MyMessage(MyMessage original) :
			base(original)
		{
			// copy() is called in superclass

            // Copy attributes
            Copy(original);
		}

		override public MessageForm CreateCopy()
		{
			return new MyMessage(this);
		}

		override protected void Copy(MessageForm message)
		{
			base.Copy(message);

			// Copy attributes
			MyMessage original = (MyMessage)message;
            Objednavka = original.Objednavka;
            Nabytok = original.Nabytok;
            Pracovisko = original.Pracovisko;
            Stolar = original.Stolar;
            StolarTypes = new Queue<StolarType>(original.StolarTypes);
		}
	}
}
