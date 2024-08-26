/// <reference types="cypress" />
 describe('Vouchers', () => {
     it('successfull voucher generation test', () => {
        const accessToken = Cypress.env('accessToken');
        const body = {
            points : 100
        };
        cy.request({
            method: 'POST',
            url: `/api/voucher`,
            body,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${accessToken}`
            },
            failOnStatusCode: false
        }).then(response => {
            cy.log('Api response: ' + JSON.stringify(response.body));
            expect(response.status).to.equal(200);
        })
     });
 })