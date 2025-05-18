export function minCoins(amount, coins) {
    if (!Array.isArray(coins)) return Infinity;
    if (typeof amount !== "number" || amount < 0) return Infinity;

    coins = coins.filter(c => Number.isInteger(c) && c > 0);

    if (amount === 0) return 0;
    if (coins.length === 0) return Infinity;

    let dp = Array(amount + 1).fill(Infinity);
    dp[0] = 0;

    for (let i = 1; i <= amount; i++) {
        for (let coin of coins) {
            if (i - coin >= 0) {
                dp[i] = dp[i - coin] + 1;
            }
        }
    }

    return dp[amount];
}
