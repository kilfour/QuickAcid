namespace QuickAcid.Tests;

public record Deposit : Act { public record Amount : Input; };
public record Withdraw : Act { public record Amount : Input; };
public record NoOverdraft : Spec;