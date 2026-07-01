namespace BalaurBohemianBroken.Stats {
    // This gets filled by the EndScreenPatch
    public class DeathIndex : StatGeneric<int> {
        public override string name => "DeathIndex";
        public override string fieldName => "ATTEMPT: ";
        
        public override float Noteworthiness() {
            // TODO: Milestones?
            return 0;
        }
    }
}