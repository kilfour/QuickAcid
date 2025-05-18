export function minCoins(amount, coins) {
    if (!Array.isArray(coins)) return Infinity;
    if (typeof amount !== "number" || amount < 0) return Infinity;

    coins = coins.filter(c => Number.isInteger(c) && c > 0).sort((a, b) => b - a);
    if (amount === 0) return 0;
    if (coins.length === 0) return Infinity;

    let count = 0;
    for (let coin of coins) {
        while (amount >= coin) {
            amount -= coin;
            count++;
        }
    }

    return amount === 0 ? count : Infinity;
}