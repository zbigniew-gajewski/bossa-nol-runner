module Registry

open Microsoft.Win32

let REGISTRYPATH = @"Software\COMARCH S.A.\NOL3\7\Settings" 
let SYNCPORTKEY = "nca_psync"
let SYNCPORTSETKEY = "ncaset_psync"
let ASYNCPORTKEY = "nca_pasync"
let ASYNCPORTSETKEY = "ncaset_pasync"   

let getRegistryValue key = 
    use registryPath = Registry.CurrentUser.OpenSubKey(REGISTRYPATH)
    match registryPath with
    | null -> failwith("Access failed to registry: hkcu\\"+REGISTRYPATH)
    | registry ->
        let portValue = registry.GetValue(key, null)
        match  portValue with
        | null -> failwith("Value not found for key: " + key)
        | value -> int (value.ToString()) 

let setRegistryValue key value = 
    use registryPath = Registry.CurrentUser.OpenSubKey(REGISTRYPATH, true)
    match registryPath with
    | null -> failwith("Access failed to registry: hkcu\\"+REGISTRYPATH)
    | registry ->
        registry.SetValue(key, value, RegistryValueKind.String)

