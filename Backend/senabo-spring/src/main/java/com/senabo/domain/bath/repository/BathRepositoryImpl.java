package com.senabo.domain.bath.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.bath.entity.Bath;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.util.List;

import static com.senabo.domain.bath.entity.QBath.bath;

@RequiredArgsConstructor
public class BathRepositoryImpl implements BathRepositoryCustom {
    private final JPAQueryFactory queryFactory;

    @Override
    public List<Bath> findByMemberId(Member member){
        return queryFactory
                .selectFrom(bath)
                .where(bath.memberId.eq(member))
                .orderBy(bath.createTime.desc())
                .fetch();
    }
}
