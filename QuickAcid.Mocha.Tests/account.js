export class Account {
    constructor() {
        this.balance = 0;
    }

    deposit(amount) {
        this.balance += amount;
    }

    withdraw(amount) {
        this.balance -= amount;
    }

    getBalance() {
        return this.balance;
    }
}

export function theAnswer() { return 42; }
export function identity(id) { return id; }
