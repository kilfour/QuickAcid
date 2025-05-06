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
