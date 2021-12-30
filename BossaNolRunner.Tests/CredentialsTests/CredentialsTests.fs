// https://mnie.github.io/2016-09-26-UnitTestsInFSharp/
// please install XUnit using Package Manager Console !!! Otherwise Test Explorer in VS will not discover tests.

namespace BossaNolRunner.Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open Types
open BossaNolRunner.NolRunner

module Specification = 

    [<Fact>]
    let ``not empty user name and not empty password validation returns Success``() =
        let credentials = ("userName", "password")
        let expected = Success credentials
        let actual = validateEmptyCredentials credentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``empty user name validation returns Failure``() =
        let credentials = ("", "password")
        let expected =  Failure (EmptyUserNameException, None)
        let actual = validateEmptyCredentials credentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``empty password validation returns Failure``() =
        let credentials = ("username", "")
        let expected = Failure (EmptyPasswordException, None)
        let actual = validateEmptyCredentials credentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``some credentials with empty username validation returns Failure``() =
        let credentials = "", "password"
        let optionalCredentials = Some credentials
        let expected = Failure (EmptyUserNameException, None)
        let actual = validateInitialCredentials optionalCredentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``some credentials with empty password validation returns Failure``() =
        let credentials = "username", ""
        let optionalCredentials = Some credentials
        let expected = Failure (EmptyPasswordException, None)
        let actual = validateInitialCredentials optionalCredentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``some not empty credentials validation returns Success``() =
        let credentials = "username", "password"
        let optionalCredentials = Some credentials
        let expected = Success credentials
        let actual = validateInitialCredentials optionalCredentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``none credentials validation returns Failure``() =
        let optionalCredentials = None
        let expected = Failure (NoInitialCredentialsException, None)
        let actual = validateInitialCredentials optionalCredentials
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``environment credentials with empty user name and empty provided credentials returns Failure `` () =
        let optionalCredentials = None
        let getCredentialsFromEnvironment = fun () -> "","password"
        let expected = Failure (EmptyUserNameException, None)
        let actual = getCredentialsFromEnvironmentVariables getCredentialsFromEnvironment optionalCredentials 
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``empty environment credentials with empty password and empty provided credentials returns Failure `` () =
        let optionalCredentials = None
        let getCredentialsFromEnvironment = fun () -> "username",""
        let expected = Failure (EmptyPasswordException, None)
        let actual = getCredentialsFromEnvironmentVariables getCredentialsFromEnvironment optionalCredentials 
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``not empty environment credentials and empty provided credentials returns Success`` () =
        let optionalCredentials = None
        let getCredentialsFromEnvironment = fun () -> "username","password"
        let expected = Success (getCredentialsFromEnvironment())
        let actual = getCredentialsFromEnvironmentVariables getCredentialsFromEnvironment optionalCredentials 
        Assert.Equal(expected, actual)
   