module Types

open System

type NolRunnerException =
    | EmptyUserNameException 
    | EmptyPasswordException 
    | NoInitialCredentialsException 
    | StartingBrowserException
    | LoginException
    | InitializingNolException

type FullException = NolRunnerException * Exception Option

type Result<'TSuccess, 'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

type UserName = string

type Password = string

type Credentials = UserName * Password
