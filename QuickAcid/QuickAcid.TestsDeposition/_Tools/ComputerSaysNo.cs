namespace QuickAcid.TestsDeposition._Tools;

//For when something really shouldn't happen, 
//and a simple throw new InvalidOperationException() just doesn't capture the emotional weight.
public class ComputerSaysNo : Exception { }

public static class ComputerSays { public static void No() { throw new ComputerSaysNo(); } }
