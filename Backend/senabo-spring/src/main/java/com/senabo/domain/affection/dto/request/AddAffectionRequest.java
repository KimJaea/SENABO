package com.senabo.domain.affection.dto.request;

import com.senabo.common.ActivityType;

public record AddAffectionRequest (
        int changeAmount,
        ActivityType type
){
}
