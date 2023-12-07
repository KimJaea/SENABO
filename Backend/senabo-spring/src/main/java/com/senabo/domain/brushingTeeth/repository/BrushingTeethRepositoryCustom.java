package com.senabo.domain.brushingTeeth.repository;

import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import com.senabo.domain.member.entity.Member;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;


@Repository
public interface BrushingTeethRepositoryCustom {
    List<BrushingTeeth> findByMemberId(Member member);
    List<BrushingTeeth> findBrushingTeethWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
    Long countBrushingTeethWeek(Member member, LocalDateTime startTime);
    Long countBrushingTeethToday(Member member);
}
