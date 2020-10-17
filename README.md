# SCPLockdown

A simple SCP SL [EXILED v2.1.7](https://github.com/galaxy119/EXILED/releases/tag/2.1.7) Plugin to lockdown SCPs at the beginning of the round for a specified amount of time.<br>

---
### Resources used

- .NET Framework 4.7.2 Class Library
- **EXILED** Library | Version 2.1.7

---
### Configs

The plugin takes max. 6 different configurable SCPs and their desired duration in seconds.  
However, when setting both of the 939 SCPs, the latter one will be the one dominating.  

Example:  

```yaml
s_c_p_lockdown:
  is_enabled: true
  # The affected SCPs and their duration of lockdown.
  affected_s_c_ps:
    Scp173: 60
    Scp096: 60
    Scp106: 60
    Scp049: 60
    Scp93989: 60
    Scp93953: 60
```
