package com.senabo.domain.brushingTeeth.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.List;

import static com.senabo.domain.brushingTeeth.entity.QBrushingTeeth.brushingTeeth;


@RequiredArgsConstructor
public class BrushingTeethRepositoryImpl implements BrushingTeethRepositoryCustom {
    private final JPAQueryFactory queryFactory;
    @Override
    public List<BrushingTeeth> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(brushingTeeth)
                .where(brushingTeeth.memberId.eq(member))
                .orderBy(brushingTeeth.createTime.desc())
                .fetch();
    }
    @Override
    public List<BrushingTeeth> findBrushingTeethWeek(Member member, LocalDateTime endTime, LocalDateTime startTime) {
        return queryFactory
                .selectFrom(brushingTeeth)
                .where(
                        brushingTeeth.memberId.eq(member),
                        brushingTeeth.updateTime.loe(endTime),
                        brushingTeeth.createTime.goe(startTime)
                )
                .orderBy(brushingTeeth.createTime.desc())
                .fetch();
    }

    @Override
    public Long countBrushingTeethWeek(Member member, LocalDateTime startTime){
        return queryFactory
                .select(brushingTeeth.count())
                .from(brushingTeeth)
                .where(
                        brushingTeeth.memberId.eq(member),
                        brushingTeeth.createTime.goe(startTime)
                )
                .fetchOne();
    }

    @Override
    public Long countBrushingTeethToday(Member member){
        return queryFactory
                .select(brushingTeeth.count())
                .from(brushingTeeth)
                .where(
                        brushingTeeth.memberId.eq(member),
                        brushingTeeth.createTime.goe(LocalDateTime.now().truncatedTo(ChronoUnit.DAYS))
                )
                .fetchOne();
    }
}
