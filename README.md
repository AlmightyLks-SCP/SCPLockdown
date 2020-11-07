# SCPLockdown

A simple SCP SL [EXILED](https://github.com/galaxy119/EXILED/) Plugin to lockdown SCPs at the beginning of the round for a specified amount of time.<br>

---
### Configs

The plugin takes max. 6 different configurable SCPs and their desired duration in seconds.  
However, when setting both of the 939 SCPs, the latter one will be the one dominating.  

Example:  

```yaml
scp_lockdown:
  is_enabled: true
  # The affected SCPs and their duration of lockdown.
  affected_scps:
    Scp173: 60
    Scp079: 60
    Scp096: 60
    Scp106: 60
    Scp049: 60
    Scp93989: 60
    Scp93953: 60
```

---
### Lockdown

#### SCP 173
Peanut's heavy gate will be locked down for the specified duration.  

#### SCP 079
Computer cannot interact with doors, elevators and such for specified duration.
(Swapping cameras excluded)

#### SCP 096
Shy Guy's door towards the HCZ will be locked down for the specified duration.  

#### SCP 106
Larry will be put in lockdown inside of his own pocket dimension for the specified duration.  

#### SCP 049
The doctor's heavy gate will be locked down for the specified duration.  

#### SCP 939 89 & 53
The two stair-entry doors will be locked down for the specified duration.  
