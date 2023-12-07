package com.senabo.domain.stress.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.stress.entity.Stress;
import com.senabo.domain.stress.entity.StressType;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface StressRepository extends JpaRepository<Stress, String>, StressRepositoryCustom {
    List<Stress> deleteByMemberId(Member member);
    List<Stress> findByMemberIdAndTypeOrderByCreateTimeDesc(Member member, StressType type);

}
