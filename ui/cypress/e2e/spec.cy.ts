describe('Home page', () => {
  beforeEach(() => {
    cy.login('cypress', 'Cypr3ss!');
  });

  it('Visits the initial project page', () => {
    cy.visit('/');
    cy.contains('Backgammon');
    cy.contains('cypress'); // logged in user visible
  });
});
