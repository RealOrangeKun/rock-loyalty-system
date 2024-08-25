/// <reference types="cypress" />
const { launchUrl } = require("../../config")

describe('Register', () => {
    it('successfull registration test', () => {
        const name = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
        const email = name + '@example.com';
        const password = 'Password123';
        const phoneNumber = Math.floor(Math.random() * 10 ** 12).toString().padStart(12, '0');
        const restaurantId = 600;
        cy.wrap(email).as('email');
        cy.wrap(password).as('password');
        cy.wrap(name).as('username');
        cy.wrap(phoneNumber).as('phoneNumber');
        const body = {
            name,
            email,
            password,
            phoneNumber,
            restaurantId
        };
        cy.request({
            method: 'POST',
            url: `${launchUrl}/api/user/register`,
            body,
            headers: {
                'Content-Type': 'application/json'
            },
            failOnStatusCode: false
        }).then(response => {
            expect(response.status).to.equal(200);
            cy.log('Registration successfull api responded with: ');
            cy.log(JSON.stringify(response.body));
        })
    })
})