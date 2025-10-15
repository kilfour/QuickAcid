 ──────────────────────────────────────────────────
 Test:                    ExampleTest
 Location:                C:\Code\QuickAcid\QuickAcid.Tests\CreateReadMe.cs:107:1
 Original failing run:    4 executions
 Minimal failing case:    1 execution (after 4 shrinks)
 Seed:                    1584314623
 ──────────────────────────────────────────────────
   => Account (tracked) : { Balance: 0 }
 ──────────────────────────────────────────────────
  Executed (3): Withdraw
   - Input: Withdraw Amount = 9
 ═══════════════════════════════
  ❌ Spec Failed: No overdraft
 ═══════════════════════════════
 Passed Specs
 - No overdraft: 3x
 ──────────────────────────────────────────────────
