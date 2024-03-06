/* eslint-disable @typescript-eslint/no-unused-vars */
// ***********************************************
// This example namespace declaration will help
// with Intellisense and code completion in your
// IDE or Text Editor.
// ***********************************************
// eslint-disable-next-line @typescript-eslint/no-namespace
declare namespace Cypress {
  interface Chainable<Subject = any> {
    login(email: string, passwor: string): typeof login;
  }

  interface Chainable<Subject = any> {
    customCommand(param: any): typeof customCommand;
  }
}

function customCommand(param: any): void {
  console.warn(param);
}
//
// NOTE: You can use it like so:
Cypress.Commands.add('customCommand', customCommand);
//
// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add("login", (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add("drag", { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add("dismiss", { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite("visit", (originalFn, url, options) => { ... })

// login and store in a cypress session

function login(email: string, password: string): void {
  cy.visit('/');
  cy.get('button').contains('Sign in').click();
  cy.get('button').contains('Password').click();

  cy.get('input[formcontrolname=name]').type(email);
  cy.get('input[formcontrolname=password]').type(password);
  cy.get('button').contains('Login').click();
}

Cypress.Commands.add('login', login);
