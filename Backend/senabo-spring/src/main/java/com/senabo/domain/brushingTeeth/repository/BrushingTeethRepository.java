package com.senabo.domain.brushingTeeth.repository;

import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;


@Repository
public interface BrushingTeethRepository extends JpaRepository<BrushingTeeth, String>, BrushingTeethRepositoryCustom {


    List<BrushingTeeth> deleteByMemberId(Member member);


}
