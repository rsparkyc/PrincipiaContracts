Like [Principia](https://github.com/mockingbirdnest/Principia)?  Want contracts utilizing Lagrange points?  Now you can have them.  This mod extends [Contract Configurator](https://github.com/jrossignol/ContractConfigurator) to let you configure contracts that can make sure your within a range of distances between two celestial bodies (a parent and a child), as well as an angle between the child body's prograde vector, and the line between the vessel and the child body (useful for differentiating between L4 and L5).

The best way to explain this is probably with some sample configs.

    CONTRACT_TYPE
    {
        # put your standard contract stuff here
        PARAMETER
        {
            name = vesselGroupLaunch
            type = VesselParameterGroup # generally, we'll want to make sure that you can maintain this Lagrange point,
                                        # so we'll combine this with a duration
            title = Reach L1

            PARAMETER
            {
                name = L1
                type = Lagrange           # This is our new paramater type
                childBodyName = Mun       # The smaller of the two bodies involevd in the Lagrang point
                parentDistance = 10000000 # The desired distance to the larger body
                parentTolerance =  100000 # The tolerance from the desired distance allowed
                childDistance = 2080000   # The desired distance to the smaller body
                childTolerance =  20000   # The tolerance from the desired distance allowed
            }
            PARAMETER
            {
                name = Duration
                type = Duration
                duration =  5d
                preWaitText = Reach L1
                waitingText = Maintaining...
                completionText = Congrats!
            }
        }
    }

    CONTRACT_TYPE
    {
        # more standard contract stuff would go here
        
        PARAMETER
        {
            name = vesselGroupLaunch
            type = VesselParameterGroup
            title = Reach L4
            
            PARAMETER
            {
                name = L4
                type = Lagrange
                childBodyName = Mun
                childDistance = 12000000
                childTolerance =  500000
                parentDistance = 12000000
                parentTolerance =  500000
                angle = 30                # Here you can see that we have an angle of 30
                                          # This makes us ahead of the moon
                angleTolerance = 5
            }
            PARAMETER
            {
                name = Duration
                type = Duration
                duration =  30d
                preWaitText = Reach L4
                waitingText = Maintaining...
                completionText = Congrats!
            }
        }
    }
    CONTRACT_TYPE
    {
        
        PARAMETER
        {
            name = vesselGroupLaunch
            type = VesselParameterGroup
            title = Reach L5
            
            PARAMETER
            {
                name = L5
                type = Lagrange
                childBodyName = Kerbin       # Here you can see the child is Kerbin, 
                                             # so the parent would be the Sun
                childDistance = 13600000000
                childTolerance =  500000000
                parentDistance = 13600000000
                parentTolerance =  500000000
                angle = 150                  # A value of 150 here means that we're behind Kerbin
                angleTolerance = 5
            }
            PARAMETER
            {
                name = Duration
                type = Duration
                duration =  30d
                preWaitText = Reach L5
                waitingText = Maintaining...
                completionText = Congrats!
            }
        }
    }

