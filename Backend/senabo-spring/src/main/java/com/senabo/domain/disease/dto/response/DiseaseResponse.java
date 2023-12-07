package com.senabo.domain.disease.dto.response;

import com.senabo.domain.disease.entity.Disease;
import lombok.Builder;

import java.time.LocalDateTime;
@Builder
public record DiseaseResponse(
        Long id,
        Long memberId,
        String diseaseName,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static DiseaseResponse from (Disease disease){
        return DiseaseResponse.builder()
                .id(disease.getId())
                .memberId(disease.getMemberId().getId())
                .diseaseName(disease.getDiseaseName())
                .createTime(disease.getCreateTime())
                .updateTime(disease.getUpdateTime())
                .build();
    }
}
